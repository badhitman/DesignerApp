////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Subscribe update - of context user
/// </summary>
public class SubscribeUpdateReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IWebRemoteTransmissionService webTransmissionRepo,
    IHelpdeskRemoteTransmissionService helpdeskTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<SubscribeUpdateRequestModel>?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SubscribeIssueUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(TAuthRequestModel<SubscribeUpdateRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool?> res = new() { Response = false };

        string[] users_ids = [req.SenderActionUserId, req.Payload.UserId];
        users_ids = [.. users_ids.Distinct()];

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.FindUsersIdentity(users_ids);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != users_ids.Length)
            return new() { Messages = rest.Messages };

        UserInfoModel
            actor = rest.Response.First(x => x.UserId == req.SenderActionUserId),
            requested_user = rest.Response.First(x => x.UserId == req.Payload.UserId);

        TResponseModel<IssueHelpdeskModelDB?> issue_data = await helpdeskTransmissionRepo.IssueRead(new TAuthRequestModel<IssueReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new IssueReadRequestModel()
            {
                IssueId = req.Payload.IssueId,
                IncludeSubscribersOnly = true,
            },
        });

        if (!issue_data.Success() || issue_data.Response is null)
            return new() { Messages = issue_data.Messages };

        if (actor.UserId != GlobalStaticConstants.Roles.System && !actor.IsAdmin && actor.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Any(y => y == x)) != true && actor.UserId != issue_data.Response.AuthorIdentityUserId)
        {
            res.AddError("У вас не достаточно прав для выполнения этой операции");
            return res;
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        var sdb = await context
             .SubscribersOfIssues
             .Where(x => x.IssueId == issue_data.Response.Id && x.UserId == requested_user.UserId)
             .Select(x => new { x.Id, x.IsSilent })
             .FirstOrDefaultAsync();

        string msg;
        if (req.Payload.SetValue)
        {
            if (sdb is null)
            {
                msg = "Подписка успешно добавлена";
                await context.SubscribersOfIssues.AddAsync(new() { UserId = requested_user.UserId, IssueId = issue_data.Response.Id, IsSilent = req.Payload.IsSilent });
                await context.SaveChangesAsync();
                res.AddSuccess(msg);

                if (req.SenderActionUserId != GlobalStaticConstants.Roles.System)
                    await helpdeskTransmissionRepo.PulsePush(new()
                    {
                        SenderActionUserId = req.SenderActionUserId,
                        Payload = new()
                        {
                            IssueId = issue_data.Response.Id,
                            PulseType = PulseIssuesTypesEnum.Subscribes,
                            Tag = GlobalStaticConstants.Routes.ADD_ACTION_NAME,
                            Description = $"Пользователь `{requested_user.UserName}` добавлен в подписчики",
                        }
                    });
            }
            else
            {
                if (req.Payload.IsSilent == sdb.IsSilent)
                    res.AddInfo("Подписка уже существует");
                else
                {
                    await context
                        .SubscribersOfIssues
                        .Where(x => x.Id == sdb.Id)
                        .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.IsSilent, req.Payload.IsSilent));
                    msg = $"Уведомления успешно {(req.Payload.IsSilent ? "отключены" : "включены")} для: {requested_user.UserName}";
                    res.AddSuccess(msg);

                    if (req.SenderActionUserId != GlobalStaticConstants.Roles.System)
                        await helpdeskTransmissionRepo.PulsePush(new()
                        {
                            SenderActionUserId = req.SenderActionUserId,
                            Payload = new()
                            {
                                IssueId = issue_data.Response.Id,
                                PulseType = PulseIssuesTypesEnum.Subscribes,
                                Tag = GlobalStaticConstants.Routes.CHANGE_ACTION_NAME,
                                Description = msg,
                            }
                        });
                }
            }
        }
        else
        {
            if (sdb is null)
                res.AddInfo("Подписки нет");
            else
            {
                await context.SubscribersOfIssues
                    .Where(x => x.Id == sdb.Id)
                    .ExecuteDeleteAsync();
                msg = "Подписка успешно удалена";
                res.AddSuccess(msg);

                if (req.SenderActionUserId != GlobalStaticConstants.Roles.System)
                    await helpdeskTransmissionRepo.PulsePush(new()
                    {
                        SenderActionUserId = req.SenderActionUserId,
                        Payload = new()
                        {
                            IssueId = issue_data.Response.Id,
                            PulseType = PulseIssuesTypesEnum.Subscribes,
                            Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                            Description = $"Пользователь `{requested_user.UserName}` удалён из подписок",
                        }
                    });
            }
        }
        res.Response = true;

        return res;
    }
}