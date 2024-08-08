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
public class GetRubricsIssuesReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<ProjectOwnedRequestModel?, RubricIssueHelpdeskModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetRubricsIssuesHelpdeskReceive;

    public async Task<TResponseModel<RubricIssueHelpdeskModelDB[]?>> ResponseHandleAction(ProjectOwnedRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<RubricIssueHelpdeskModelDB[]?> res = new();

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        IQueryable<RubricIssueHelpdeskModelDB> q = context
            .RubricsForIssues
            .Where(x => x.ParentRubricId == req.OwnerId && x.ProjectId == req.ProjectId);
        res.Response = await q.ToArrayAsync();
        return res;
    }
}