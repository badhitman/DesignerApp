////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// AddNewMessageIntoIssue
/// </summary>
public class MessageForIssueUpdateOrCreateReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<IssueMessageHelpdeskBaseModel?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MessageOfIssueUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(IssueMessageHelpdeskBaseModel? payload)
    {
        TResponseModel<int?> res = new();
        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        return res;
    }
}