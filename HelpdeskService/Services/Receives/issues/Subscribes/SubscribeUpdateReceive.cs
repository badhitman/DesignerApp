////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Subscribe update - of context user
/// </summary>
public class SubscribeUpdateReceive(IHelpdeskService hdRepo)
    : IResponseReceive<TAuthRequestModel<SubscribeUpdateRequestModel>?, TResponseModel<bool?>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SubscribeIssueUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>?> ResponseHandleAction(TAuthRequestModel<SubscribeUpdateRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await hdRepo.SubscribeUpdate(req);
    }
}