////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// ArticlesSelectReceive
/// </summary>
public class ArticlesSelectReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<TPaginationRequestModel<SelectArticlesRequestModel>?, TPaginationResponseModel<ArticleModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ArticlesSelectHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<ArticleModelDB>?>> ResponseHandleAction(TPaginationRequestModel<SelectArticlesRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        if (req.PageSize < 5)
            req.PageSize = 5;

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IQueryable<ArticleModelDB> q = context
            .Articles
            .Where(x => x.ProjectId == req.Payload.ProjectId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
        {
            req.Payload.SearchQuery = req.Payload.SearchQuery.ToUpper();

            //q = from issue_element in q
            //    join rubric_element in context.RubricsForIssues on issue_element.RubricIssueId equals rubric_element.Id
            //    into grp_rubrics
            //    from c in grp_rubrics.DefaultIfEmpty()
            //    where issue_element.NormalizedNameUpper!.Contains(req.Payload.SearchQuery) || c.NormalizedNameUpper!.Contains(req.Payload.SearchQuery)
            //    select issue_element;
        }

        var inc = q
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize)
            .Include(x => x.Tags);

        return new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = [.. req.Payload.IncludeTags ? await inc.Include(x => x.Tags).ToListAsync() : await inc.ToListAsync()]
            }
        };
    }
}