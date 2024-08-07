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
    : IResponseReceive<object?, IssueThemeHelpdeskModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetThemesIssuesHelpdeskReceive;

    public Task<TResponseModel<IssueThemeHelpdeskModelDB[]?>> ResponseHandleAction(object? payload)
    {
        TResponseModel<IssueThemeHelpdeskModelDB[]?> res = new();

        return Task.FromResult(res);
    }
}