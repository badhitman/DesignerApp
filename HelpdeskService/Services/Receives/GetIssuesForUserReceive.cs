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
    : IResponseReceive<(long? telegramId, string? identityId)?, IssueModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetIssuesForUserHelpdeskReceive;

    public Task<TResponseModel<IssueModelDB[]?>> ResponseHandleAction((long? telegramId, string? identityId)? for_user)
    {
        TResponseModel<IssueModelDB[]?> res = new();

        return Task.FromResult(res);
    }
}