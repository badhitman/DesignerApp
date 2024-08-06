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
    : IResponseReceive<object?, IssueThemeModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetThemesIssuesHelpdeskReceive;

    public Task<TResponseModel<IssueThemeModelDB[]?>> ResponseHandleAction(object? payload)
    {
        TResponseModel<IssueThemeModelDB[]?> res = new();

        return Task.FromResult(res);
    }
}