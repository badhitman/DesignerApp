////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// MessageVote Receive
/// </summary>
public class MessageVoteReceive(IHelpdeskService hdRepo)
    : IResponseReceive<TAuthRequestModel<VoteIssueRequestModel>?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MessageOfIssueVoteHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(TAuthRequestModel<VoteIssueRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await hdRepo.MessageVote(req);
    }
}