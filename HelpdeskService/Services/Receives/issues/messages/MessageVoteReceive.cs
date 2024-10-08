﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;
using Newtonsoft.Json;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// MessageVoteReceive
/// </summary>
public class MessageVoteReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    ILogger<MessageVoteReceive> loggerRepo,
    IWebRemoteTransmissionService webTransmissionRepo,
    IHelpdeskRemoteTransmissionService helpdeskTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<VoteIssueRequestModel>?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MessageOfIssueVoteHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(TAuthRequestModel<VoteIssueRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool?> res = new();
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<UserInfoModel[]?> rest = req.SenderActionUserId == GlobalStaticConstants.Roles.System
            ? new() { Response = [UserInfoModel.BuildSystem()] }
            : await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);

        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        IssueMessageHelpdeskModelDB msg_db = await context.IssuesMessages.FirstAsync(x => x.Id == req.Payload.MessageId);

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await helpdeskTransmissionRepo.IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssuesIds = [msg_db.IssueId], IncludeSubscribersOnly = true },
        });

        if (!issues_data.Success() || issues_data.Response is null || issues_data.Response.Length != 1)
            return new() { Messages = issues_data.Messages };

        if (!actor.IsAdmin && actor.UserId != GlobalStaticConstants.Roles.System && actor.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Any(y => y == x)) != true && !issues_data.Response.All(iss => actor.UserId == iss.AuthorIdentityUserId))
        {
            res.AddError("У вас не достаточно прав");
            return res;
        }
        var issue_data = issues_data.Response.Single();
        if (req.SenderActionUserId != GlobalStaticConstants.Roles.System && issue_data.Subscribers?.Any(x => x.UserId == req.SenderActionUserId) != true)
        {
            await helpdeskTransmissionRepo.SubscribeUpdate(new()
            {
                SenderActionUserId = GlobalStaticConstants.Roles.System,
                Payload = new()
                {
                    IssueId = issue_data.Id,
                    SetValue = true,
                    UserId = actor.UserId,
                    IsSilent = false,
                }
            });
        }

        int? vote_db_key = await context
            .Votes
            .Where(x => x.MessageId == msg_db.Id && x.IdentityUserId == req.SenderActionUserId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
        PulseRequestModel p_req;
        if (req.Payload.SetStatus)
        {
            if (!vote_db_key.HasValue)
            {
                VoteHelpdeskModelDB vote_db = new() { IdentityUserId = actor.UserId, IssueId = issue_data.Id, MessageId = msg_db.Id };
                await context.AddAsync(vote_db);
                await context.SaveChangesAsync();

                res.AddSuccess("Ваш голос учтён");
                p_req = new()
                {
                    Payload = new()
                    {
                        SenderActionUserId = req.SenderActionUserId,
                        Payload = new()
                        {
                            IssueId = issue_data.Id,
                            PulseType = PulseIssuesTypesEnum.Vote,
                            Tag = GlobalStaticConstants.Routes.ADD_ACTION_NAME,
                            Description = $"Пользователь `{actor.UserName}` проголосовал за сообщение #{msg_db.Id}",
                        }
                    }
                };

                await helpdeskTransmissionRepo.PulsePush(p_req);
            }
            else
                res.AddInfo("Вы уже проголосовали");
        }
        else
        {
            if (!vote_db_key.HasValue)
                res.AddInfo("Ваш голос отсутствует");
            else
            {
                await context
                    .Votes
                    .Where(x => x.Id == vote_db_key.Value)
                    .ExecuteDeleteAsync();

                res.AddInfo("Ваш голос удалён");
                p_req = new()
                {
                    Payload = new()
                    {
                        SenderActionUserId = req.SenderActionUserId,
                        Payload = new()
                        {
                            IssueId = issue_data.Id,
                            PulseType = PulseIssuesTypesEnum.Vote,
                            Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                            Description = $"Пользователь `{actor.UserName}` удалил свой голос за сообщение #{msg_db.Id}",
                        }
                    }
                };

                await helpdeskTransmissionRepo.PulsePush(p_req);
            }
        }

        return res;
    }
}