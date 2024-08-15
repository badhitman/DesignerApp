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
public class SubscribeUpdateReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory, IWebRemoteTransmissionService webTransmissionRepo, IHelpdeskRemoteTransmissionService helpdeskTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<SubscribeUpdateRequestModel>?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SubscribeIssueUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(TAuthRequestModel<SubscribeUpdateRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool?> res = new();

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.FindUsersIdentity([req.SenderActionUserId, req.Payload.UserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 2)
            return new() { Messages = rest.Messages };

        UserInfoModel
            actor = rest.Response.First(x => x.UserId == req.SenderActionUserId),
            requested_user = rest.Response.First(x => x.UserId == req.Payload.UserId);

        TResponseModel<IssueHelpdeskModelDB?> issue_data = await helpdeskTransmissionRepo.IssueRead(new TAuthRequestModel<int>()
        {
            SenderActionUserId = req.SenderActionUserId,
            Payload = req.Payload.IssueId,
        });

        if (!issue_data.Success() || issue_data.Response is null)
            return new() { Messages = issue_data.Messages };

        string[] hd_roles = [GlobalStaticConstants.Roles.HelpDeskTelegramBotManager, GlobalStaticConstants.Roles.HelpDeskTelegramBotUnit];

        if (actor.Roles?.Any(x => hd_roles.Any(y => y == x)) != true && actor.UserId != issue_data.Response.AuthorIdentityUserId)
        {
            res.AddError("У вас не достаточно прав для выполнения этой операции");
            return res;
        }

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        int? sdb = await context
             .SubscribersOfIssues
             .Where(x => x.IssueId == issue_data.Response.Id && x.UserId == requested_user.UserId)
             .Select(x => x.Id)
             .FirstOrDefaultAsync();

        if (req.Payload.SubscribeSet)
        {
            if (!sdb.HasValue || sdb.Value == default)
            {
                await context.SubscribersOfIssues.AddAsync(new() { UserId = requested_user.UserId, IssueId = issue_data.Response.Id, });
                await context.SaveChangesAsync();
                res.AddSuccess("Подписка успешно добавлена");
            }
            else
                res.AddInfo("Подписка уже существует");
        }
        else
        {
            if (sdb is null)
                res.AddInfo("Подписка уже существует");
            else
            {
                await context.SubscribersOfIssues
                    .Where(x => x.Id == sdb.Value)
                    .ExecuteDeleteAsync();

                res.AddSuccess("Подписка успешно удалена");
            }
        }

        return res;
    }
}