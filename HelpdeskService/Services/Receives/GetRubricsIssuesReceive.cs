////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// GetThemesIssues
/// </summary>
public class GetRubricsIssuesReceive
    : IResponseReceive<ProjectOwnedRequestModel?, RubricIssueHelpdeskModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetRubricsIssuesHelpdeskReceive;

    public Task<TResponseModel<RubricIssueHelpdeskModelDB[]?>> ResponseHandleAction(ProjectOwnedRequestModel? req)
    {
        TResponseModel<RubricIssueHelpdeskModelDB[]?> res = new();

        return Task.FromResult(res);
    }
}