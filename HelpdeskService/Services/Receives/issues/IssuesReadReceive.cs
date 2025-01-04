////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Read issue - of context user
/// </summary>
public class IssuesReadReceive(IHelpdeskService hdRepo)
    : IResponseReceive<TAuthRequestModel<IssuesReadRequestModel>?, IssueHelpdeskModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssuesGetHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB[]?>> ResponseHandleAction(TAuthRequestModel<IssuesReadRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<IssueHelpdeskModelDB[]> res = await hdRepo.IssuesRead(req);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages
        };
    }
}