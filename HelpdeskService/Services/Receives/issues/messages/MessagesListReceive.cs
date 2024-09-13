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

        TResponseModel<IssueHelpdeskModelDB> issue_data = await helpdeskTransmissionRepo.IssueRead(new TAuthRequestModel<IssueReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssueId = req.Payload, IncludeSubscribersOnly = true },
        });

        if (!issue_data.Success() || issue_data.Response is null)
            return new() { Messages = issue_data.Messages };

        if (!actor.IsAdmin && actor.UserId != GlobalStaticConstants.Roles.System && actor.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Any(y => y == x)) != true && actor.UserId != issue_data.Response.AuthorIdentityUserId)
        {
            res.AddError("У вас не достаточно прав");
            return res;
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        res.Response = await context
            .IssuesMessages
            .Include(x => x.Votes)
            .Where(x => x.IssueId == issue_data.Response.Id)
            .ToArrayAsync();

        return res;
    }
}