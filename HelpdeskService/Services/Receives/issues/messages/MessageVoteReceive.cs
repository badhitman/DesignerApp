////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// MessageVoteReceive
/// </summary>
public class MessageVoteReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
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

        TResponseModel<UserInfoModel[]?> rest = req.SenderActionUserId == GlobalStaticConstants.Roles.System
            ? new() { Response = [UserInfoModel.BuildSystem()] }
            : await webTransmissionRepo.FindUsersIdentity([req.SenderActionUserId]);

        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        IssueMessageHelpdeskModelDB msg_db = await context.IssuesMessages.FirstAsync(x => x.Id == req.Payload.MessageId);

        TResponseModel<IssueHelpdeskModelDB?> issue_data = await helpdeskTransmissionRepo.IssueRead(new TAuthRequestModel<IssueReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssueId = msg_db.IssueId, IncludeSubscribersOnly = true },
        });

        if (!issue_data.Success() || issue_data.Response is null)
            return new() { Messages = issue_data.Messages };

        if (!actor.IsAdmin && actor.UserId != GlobalStaticConstants.Roles.System && actor.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Any(y => y == x)) != true && actor.UserId != issue_data.Response.AuthorIdentityUserId)
        {
            res.AddError("У вас не достаточно прав");
            return res;
        }

        if (req.SenderActionUserId != GlobalStaticConstants.Roles.System && issue_data.Response.Subscribers?.Any(x => x.UserId == req.SenderActionUserId) != true)
        {
            await helpdeskTransmissionRepo.SubscribeUpdate(new()
            {
                SenderActionUserId = GlobalStaticConstants.Roles.System,
                Payload = new()
                {
                    IssueId = issue_data.Response.Id,
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

        if (req.Payload.SetStatus)
        {
            if (!vote_db_key.HasValue)
            {
                VoteHelpdeskModelDB vote_db = new() { IdentityUserId = actor.UserId, IssueId = issue_data.Response.Id, MessageId = msg_db.Id };
                await context.AddAsync(vote_db);
                await context.SaveChangesAsync();

                res.AddSuccess("Ваш голос учтён");
                await helpdeskTransmissionRepo.PulsePush(new()
                {
                    SenderActionUserId = req.SenderActionUserId,
                    Payload = new()
                    {
                        IssueId = issue_data.Response.Id,
                        PulseType = PulseIssuesTypesEnum.Vote,
                        Tag = GlobalStaticConstants.Routes.ADD_ACTION_NAME,
                        Description = $"Пользователь `{actor.UserName}` проголосовал за сообщение #{msg_db.Id}",
                    }
                });
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
                await helpdeskTransmissionRepo.PulsePush(new()
                {
                    SenderActionUserId = req.SenderActionUserId,
                    Payload = new()
                    {
                        IssueId = issue_data.Response.Id,
                        PulseType = PulseIssuesTypesEnum.Vote,
                        Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                        Description = $"Пользователь `{actor.UserName}` удалил свой голос за сообщение #{msg_db.Id}",
                    }
                });
            }
        }

        return res;
    }
}