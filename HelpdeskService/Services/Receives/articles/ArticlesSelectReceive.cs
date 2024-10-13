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
            //q = q.Where(x => x.NormalizedNameUpper != null && x.NormalizedNameUpper.Contains(req.Payload.SearchQuery));

            q = from article_element in q 
                join rj_element in context.RubricsArticlesJoins on article_element.Id equals rj_element.ArticleId into outer_rj
                from rj_item in outer_rj.DefaultIfEmpty()
                join rubric_element in context.RubricsForIssues on rj_item.RubricId equals rubric_element.Id into outer_rubric
                from rubric_item in outer_rubric.DefaultIfEmpty()
                where article_element.NormalizedNameUpper!.Contains(req.Payload.SearchQuery) || rubric_item.NormalizedNameUpper!.Contains(req.Payload.SearchQuery)
                select article_element;

        }

        var oq = req.SortingDirection == VerticalDirectionsEnum.Up
          ? q.OrderBy(x => x.UpdatedAtUTC).Skip(req.PageNum * req.PageSize).Take(req.PageSize)
          : q.OrderByDescending(x => x.UpdatedAtUTC).Skip(req.PageNum * req.PageSize).Take(req.PageSize);


        var inc = oq
            .Include(x => x.RubricsJoins)
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
                Response = [.. req.Payload.IncludeExternal ? await inc.ToListAsync() : await oq.ToListAsync()]
            }
        };
    }
}