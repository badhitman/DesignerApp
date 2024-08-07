////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// CreateIssue
/// </summary>
public class CreateIssueReceive
    : IResponseReceive<IssueHelpdeskModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CreateIssueHelpdeskReceive;

    public Task<TResponseModel<int?>> ResponseHandleAction(IssueHelpdeskModelDB? payload)
    {
        TResponseModel<int?> res = new();

        return Task.FromResult(res);
    }
}