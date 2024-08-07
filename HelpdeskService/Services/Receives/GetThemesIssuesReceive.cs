////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// GetThemesIssues
/// </summary>
public class GetThemesIssuesReceive
    : IResponseReceive<object?, RubricIssueHelpdeskModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetThemesIssuesHelpdeskReceive;

    public Task<TResponseModel<RubricIssueHelpdeskModelDB[]?>> ResponseHandleAction(object? payload)
    {
        TResponseModel<RubricIssueHelpdeskModelDB[]?> res = new();

        return Task.FromResult(res);
    }
}