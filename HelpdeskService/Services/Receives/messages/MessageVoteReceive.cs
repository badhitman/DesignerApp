////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// GetThemesIssues
/// </summary>
public class MessageVoteReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<VoteIssueRequestModel?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MessageOfIssueSetAsResponseHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(VoteIssueRequestModel? payload)
    {
        TResponseModel<bool?> res = new();
        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        return res;
    }
}