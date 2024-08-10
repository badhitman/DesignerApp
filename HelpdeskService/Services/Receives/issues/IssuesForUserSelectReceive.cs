////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// GetIssuesForUser
/// </summary>
public class IssuesForUserSelectReceive
    : IResponseReceive<GetIssuesForUserRequestModel?, IssueHelpdeskModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssuesSelectHelpdeskReceive;

    /// <inheritdoc/>
    public Task<TResponseModel<IssueHelpdeskModelDB[]?>> ResponseHandleAction(GetIssuesForUserRequestModel? req)
    {
        TResponseModel<IssueHelpdeskModelDB[]?> res = new();

        return Task.FromResult(res);
    }
}