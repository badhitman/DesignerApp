////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Получить сообщения для инцидента
/// </summary>
public class MessagesListReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IWebRemoteTransmissionService webTransmissionRepo,
    IHelpdeskRemoteTransmissionService helpdeskTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<int>?, IssueMessageHelpdeskModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MessagesOfIssueListHelpdeskReceive;

    /// <summary>
    /// Получить сообщения для инцидента
    /// </summary>
    public async Task<TResponseModel<IssueMessageHelpdeskModelDB[]?>> ResponseHandleAction(TAuthRequestModel<int>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<IssueMessageHelpdeskModelDB[]?> res = new();

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await helpdeskTransmissionRepo.IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssuesIds = [req.Payload], IncludeSubscribersOnly = true },
        });

        if (!issues_data.Success() || issues_data.Response is null)
            return new() { Messages = issues_data.Messages };

        if (!actor.IsAdmin && actor.UserId != GlobalStaticConstants.Roles.System && actor.Roles?.Any(role_actor => GlobalStaticConstants.Roles.AllHelpDeskRoles.Any(hd_role => hd_role == role_actor)) != true && !issues_data.Response.All(c => actor.UserId == c.AuthorIdentityUserId))
        {
            res.AddError("У вас не достаточно прав");
            return res;
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        int[] issues_ids = issues_data.Response.Select(i => i.Id).ToArray();

        res.Response = await context
            .IssuesMessages
            .Where(x => issues_ids.Any(y => y == x.IssueId))
            .OrderByDescending(i => i.CreatedAt)
            .Include(x => x.Votes)
            .ToArrayAsync();

        return res;
    }
}