////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Read issue - of context user
/// </summary>
public class IssueReadReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<IssueReadRequestModel?, IssueHelpdeskModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssueGetHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB?>> ResponseHandleAction(IssueReadRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<IssueHelpdeskModelDB?> res = new();

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        //IQueryable<IssueHelpdeskModelDB> q = context
        //    .Issues
        //    .Where(x => x.Id == req.IssueId)
        //    .AsQueryable();

        return res;
    }
}