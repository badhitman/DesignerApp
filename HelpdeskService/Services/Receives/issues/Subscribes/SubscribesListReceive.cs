////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Subscribes list - of context user
/// </summary>
public class SubscribesListReceive(
    IWebRemoteTransmissionService webTransmissionRepo,
    IHelpdeskRemoteTransmissionService helpdeskTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<int>?, SubscriberIssueHelpdeskModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SubscribesIssueListHelpdeskReceive;

    /// <summary>
    /// Подписчики на события в обращении/инциденте
    /// </summary>
    public async Task<TResponseModel<SubscriberIssueHelpdeskModelDB[]?>> ResponseHandleAction(TAuthRequestModel<int>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<SubscriberIssueHelpdeskModelDB[]?> res = new();
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

        res.Response = [.. issues_data.Response.SelectMany(x => x.Subscribers!)];
        return res;
    }
}