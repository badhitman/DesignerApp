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
    : IResponseReceive<IssueModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CreateIssueHelpdeskReceive;

    public Task<TResponseModel<int?>> ResponseHandleAction(IssueModelDB? payload)
    {
        TResponseModel<int?> res = new();

        return Task.FromResult(res);
    }
}