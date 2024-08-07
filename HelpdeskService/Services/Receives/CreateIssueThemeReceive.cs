////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// CreateIssueTheme
/// </summary>
public class CreateIssueThemeReceive
    : IResponseReceive<IssueThemeHelpdeskModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CreateIssuesThemeHelpdeskReceive;

    public Task<TResponseModel<int?>> ResponseHandleAction(IssueThemeHelpdeskModelDB? payload)
    {
        TResponseModel<int?> res = new();

        return Task.FromResult(res);
    }
}