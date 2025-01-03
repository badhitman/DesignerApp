////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Subscribe update - of context user
/// </summary>
public class ExecuterUpdateReceive(IHelpdeskService hdRepo) 
    : IResponseReceive<TAuthRequestModel<UserIssueModel>, TResponseModel<bool>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ExecuterIssueUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>?> ResponseHandleAction(TAuthRequestModel<UserIssueModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await hdRepo.ExecuterUpdate(req);
    }
}