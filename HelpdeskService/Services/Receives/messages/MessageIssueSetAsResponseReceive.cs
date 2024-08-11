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
public class MessageIssueSetAsResponseReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<SetMessageAsResponseIssueRequestModel?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MessageOfIssueSetAsResponseHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(SetMessageAsResponseIssueRequestModel? payload)
    {
        TResponseModel<bool?> res = new();
        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        return res;
    }
}