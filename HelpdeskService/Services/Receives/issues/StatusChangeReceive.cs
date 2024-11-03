////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// StatusChangeReceive
/// </summary>
public class StatusChangeReceive(IHelpdeskService hdRepo)
    : IResponseReceive<TAuthRequestModel<StatusChangeRequestModel>?, bool>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.StatusChangeIssueHelpdeskReceive;
    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ResponseHandleAction(TAuthRequestModel<StatusChangeRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await hdRepo.StatusChange(req);
    }
}