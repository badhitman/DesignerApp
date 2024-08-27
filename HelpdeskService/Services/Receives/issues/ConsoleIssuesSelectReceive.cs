////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// ConsoleIssuesSelectReceive
/// </summary>
public class ConsoleIssuesSelectReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<TPaginationRequestModel<ConsoleIssuesRequestModel>?, TPaginationResponseModel<IssueHelpdeskModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ConsoleIssuesSelectHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>?>> ResponseHandleAction(TPaginationRequestModel<ConsoleIssuesRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        if (req.PageSize < 5)
            req.PageSize = 5;

        TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>?> res = new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
            }
        };

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IQueryable<IssueHelpdeskModelDB> q = context
            .Issues
            .Where(x => x.ProjectId == req.Payload.ProjectId && x.StepIssue == req.Payload.Status)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
        {
            req.Payload.SearchQuery = req.Payload.SearchQuery.ToUpper();
            q = q.Where(x => x.NormalizedNameUpper!.Contains(req.Payload.SearchQuery));
        }

        res.Response.TotalRowsCount = await q.CountAsync();

        List<IssueHelpdeskModelDB> data = await q
            .OrderBy(x => x.CreatedAt)
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize)
            .Include(x => x.RubricIssue)
            .ToListAsync();

        res.Response.Response = data.Select(x => IssueHelpdeskModel.Build(x)).ToList();

        return res;
    }
}