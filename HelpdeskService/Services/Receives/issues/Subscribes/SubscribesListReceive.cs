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
    : IResponseReceive<TAuthRequestModel<int>, SubscriberIssueHelpdeskModelDB[]>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SubscribesIssueListHelpdeskReceive;

    /// <summary>
    /// Подписчики на события в обращении/инциденте
    /// </summary>
    public async Task<SubscriberIssueHelpdeskModelDB[]?> ResponseHandleAction(TAuthRequestModel<int>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return [];

        UserInfoModel actor = rest.Response[0];

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await helpdeskTransmissionRepo.IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssuesIds = [req.Payload], IncludeSubscribersOnly = true },
        });

        if (!issues_data.Success() || issues_data.Response is null)
            return [];

        return [.. issues_data.Response.SelectMany(x => x.Subscribers!)];
    }
}