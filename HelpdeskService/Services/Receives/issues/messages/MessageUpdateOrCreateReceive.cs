////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Сообщение в обращение
/// </summary>
public class MessageUpdateOrCreateReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IWebRemoteTransmissionService webTransmissionRepo,
    IHelpdeskRemoteTransmissionService helpdeskTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<IssueMessageHelpdeskBaseModel>?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MessageOfIssueUpdateHelpdeskReceive;

    /// <summary>
    /// Сообщение в обращение
    /// </summary>
    public async Task<TResponseModel<int?>> ResponseHandleAction(TAuthRequestModel<IssueMessageHelpdeskBaseModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<int?> res = new();

        if (string.IsNullOrWhiteSpace(req.Payload.MessageText))
        {
            res.AddError("Пустой текст сообщения");
            return res;
        }
        req.Payload.MessageText = req.Payload.MessageText.Trim();

        TResponseModel<IssueHelpdeskModelDB?> issue_data = await helpdeskTransmissionRepo.IssueRead(new TAuthRequestModel<IssueReadRequestModel>()
        {
            SenderActionUserId = req.SenderActionUserId,
            Payload = new() { IssueId = req.Payload.IssueId, IncludeSubscribersOnly = true },
        });

        if (!issue_data.Success() || issue_data.Response is null)
            return new() { Messages = issue_data.Messages };

        TResponseModel<UserInfoModel[]?> rest = req.SenderActionUserId == GlobalStaticConstants.Roles.System
            ? new() { Response = [UserInfoModel.BuildSystem()] }
            : await webTransmissionRepo.FindUsersIdentity([req.SenderActionUserId]);

        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

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

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        IssueMessageHelpdeskModelDB msg_db;
        IssueReadMarkerHelpdeskModelDB? my_marker;
        DateTime dtn = DateTime.UtcNow;
        string msg;
        if (req.Payload.Id < 1)
        {
            msg_db = new()
            {
                AuthorUserId = actor.UserId,
                CreatedAt = dtn,
                LastUpdateAt = dtn,
                MessageText = req.Payload.MessageText,
                IssueId = req.Payload.IssueId,
            };
            msg = "Сообщение успешно отправлено";
            await context.AddAsync(msg_db);
            await context.SaveChangesAsync();
            res.AddSuccess(msg);

            res.Response = msg_db.Id;
            if (actor.UserId != GlobalStaticConstants.Roles.System)
            {
                await helpdeskTransmissionRepo.PulsePush(new()
                {
                    SenderActionUserId = req.SenderActionUserId,
                    Payload = new()
                    {
                        IssueId = issue_data.Response.Id,
                        PulseType = PulseIssuesTypesEnum.Messages,
                        Tag = GlobalStaticConstants.Routes.ADD_ACTION_NAME,
                        Description = $"Пользователь `{actor.UserName}` добавил комментарий в обращение #{issue_data.Response.Id} '{issue_data.Response.Name}'",
                    }
                });

                my_marker = await context.IssueReadMarkers.FirstOrDefaultAsync(x => x.IssueId == req.Payload.IssueId && x.UserIdentityId == actor.UserId);
                if (my_marker is null)
                {
                    my_marker = new()
                    {
                        LastReadAt = dtn,
                        UserIdentityId = actor.UserId,
                        IssueId = req.Payload.IssueId,
                    };
                    await context.AddAsync(my_marker);
                    await context.SaveChangesAsync();
                }
                else
                {
                    await context.IssueReadMarkers.Where(x => x.Id == my_marker.Id)
                        .ExecuteUpdateAsync(set => set
                        .SetProperty(p => p.LastReadAt, dtn));
                }

                await context
                    .IssueReadMarkers
                    .Where(x => x.IssueId == req.Payload.IssueId && x.Id != my_marker.Id)
                    .ExecuteDeleteAsync();
            }
            else
                await context
                .IssueReadMarkers
                .Where(x => x.IssueId == req.Payload.IssueId)
                .ExecuteDeleteAsync();
        }
        else
        {
            res.Response = 0;
            msg_db = await context.IssuesMessages.FirstAsync(x => x.Id == req.Payload.Id);

            if (msg_db.MessageText == req.Payload.MessageText)
                res.AddInfo("Изменений нет");
            else if (!actor.IsAdmin && msg_db.AuthorUserId != actor.UserId)
            {
                res.AddError("Не достаточно прав");
            }
            else
            {
                await helpdeskTransmissionRepo.PulsePush(new()
                {
                    SenderActionUserId = req.SenderActionUserId,
                    Payload = new()
                    {
                        IssueId = issue_data.Response.Id,
                        PulseType = PulseIssuesTypesEnum.Messages,
                        Tag = GlobalStaticConstants.Routes.CHANGE_ACTION_NAME,
                        Description = $"Пользователь `{actor.UserName}` изменил комментарий #{msg_db.Id}.",
                    }
                });

                res.Response = await context
                    .IssuesMessages
                    .Where(x => x.Id == msg_db.Id)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.MessageText, req.Payload.MessageText)
                    .SetProperty(p => p.LastUpdateAt, dtn));

                msg = "Сообщение успешно обновлено";
                res.AddSuccess(msg);
            }
        }

        return res;
    }
}