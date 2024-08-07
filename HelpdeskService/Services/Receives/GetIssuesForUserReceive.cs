////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// GetIssuesForUser
/// </summary>
public class GetIssuesForUserReceive
    : IResponseReceive<GetIssuesForUserRequestModel?, IssueHelpdeskModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetIssuesForUserHelpdeskReceive;

    public Task<TResponseModel<IssueHelpdeskModelDB[]?>> ResponseHandleAction(GetIssuesForUserRequestModel? req)
    {
        TResponseModel<IssueHelpdeskModelDB[]?> res = new();

        return Task.FromResult(res);
    }
}