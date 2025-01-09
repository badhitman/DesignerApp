////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// IssuesSelectReceive
/// </summary>
public class IssuesSelectReceive(IHelpdeskService hdRepo)
    : IResponseReceive<TAuthRequestModel<TPaginationRequestModel<SelectIssuesRequestModel>>?, TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssuesSelectHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>>?> ResponseHandleAction(TAuthRequestModel<TPaginationRequestModel<SelectIssuesRequestModel>>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await hdRepo.IssuesSelect(req);
    }
}