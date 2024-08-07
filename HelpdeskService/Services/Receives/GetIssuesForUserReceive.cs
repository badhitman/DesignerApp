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
    : IResponseReceive<UserCrossIdsModel?, IssueHelpdeskModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetIssuesForUserHelpdeskReceive;

    public Task<TResponseModel<IssueHelpdeskModelDB[]?>> ResponseHandleAction(UserCrossIdsModel? for_user)
    {
        TResponseModel<IssueHelpdeskModelDB[]?> res = new();

        return Task.FromResult(res);
    }
}