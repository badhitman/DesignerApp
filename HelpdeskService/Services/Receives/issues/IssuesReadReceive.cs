////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Read issue - of context user
/// </summary>
public class IssuesReadReceive(IHelpdeskService hdRepo) : IResponseReceive<TAuthRequestModel<IssuesReadRequestModel>?, TResponseModel<IssueHelpdeskModelDB[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssuesGetHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB[]>?> ResponseHandleAction(TAuthRequestModel<IssuesReadRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await hdRepo.IssuesRead(req);
    }
}