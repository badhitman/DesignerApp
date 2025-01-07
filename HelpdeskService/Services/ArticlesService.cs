////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SharedLib;
using DbcLib;

namespace HelpdeskService;

/// <summary>
/// Articles
/// </summary>
public class ArticlesService(IDbContextFactory<HelpdeskContext> helpdeskDbFactory) : IArticlesService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<int>> ArticleCreateOrUpdate(ArticleModelDB article)
    {
        TResponseModel<int> res = new TResponseModel<int>();
        Regex rx = new(@"\s+", RegexOptions.Compiled);
        article.Name = rx.Replace(article.Name.Trim(), " ");
        if (string.IsNullOrWhiteSpace(article.Name))
        {
            res.AddError("Укажите название");
            return res;
        }

        article.NormalizedNameUpper = article.Name.ToUpper();
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        DateTime dtu = DateTime.UtcNow;
        if (article.Id < 1)
        {
            article.Id = 0;
            article.CreatedAtUTC = dtu;

            await context.AddAsync(article);
            await context.SaveChangesAsync();
            res.Response = article.Id;
            res.AddSuccess("Статья успешно создана");
            return res;
        }
        res.Response = await context.Articles
            .Where(a => a.Id == article.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.LastUpdatedAtUTC, dtu)
            .SetProperty(p => p.Name, article.Name)
            .SetProperty(p => p.Description, article.Description)
            .SetProperty(p => p.NormalizedNameUpper, article.NormalizedNameUpper));

        if (res.Response < 1)
            res.AddInfo("Запрос не вызвал измений в БД");

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<ArticleModelDB[]>> ArticlesRead(int[] req)
    {
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
#if DEBUG
        var _res = await context.Articles
            .Where(x => req.Any(y => y == x.Id))
            .Include(x => x.RubricsJoins!)
            .ThenInclude(x => x.Rubric)
            .ToArrayAsync();
#endif
        return new()
        {
            Response = await context.Articles
            .Where(x => req.Any(y => y == x.Id))
            .Include(x => x.RubricsJoins!)
            .ThenInclude(x => x.Rubric)
            .ToArrayAsync()
        };
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<ArticleModelDB>> ArticlesSelect(TPaginationRequestModel<SelectArticlesRequestModel> req)
    {
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

            q = from article_element in q
                join rj_element in context.RubricsArticlesJoins on article_element.Id equals rj_element.ArticleId into outer_rj
                from rj_item in outer_rj.DefaultIfEmpty()
                join rubric_element in context.Rubrics on rj_item.RubricId equals rubric_element.Id into outer_rubric
                from rubric_item in outer_rubric.DefaultIfEmpty()
                where article_element.NormalizedNameUpper!.Contains(req.Payload.SearchQuery) || rubric_item.NormalizedNameUpper!.Contains(req.Payload.SearchQuery)
                select article_element;
        }

        IQueryable<ArticleModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
          ? q.OrderBy(x => x.LastUpdatedAtUTC).ThenBy(x => x.CreatedAtUTC).Skip(req.PageNum * req.PageSize).Take(req.PageSize)
          : q.OrderByDescending(x => x.LastUpdatedAtUTC).ThenByDescending(x => x.CreatedAtUTC).Skip(req.PageNum * req.PageSize).Take(req.PageSize);

        var inc = oq
            .Include(x => x.RubricsJoins)
            ;

        return new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortingDirection = req.SortingDirection,
            SortBy = req.SortBy,
            TotalRowsCount = await q.CountAsync(),
            Response = [.. req.Payload.IncludeExternal ? await inc.ToListAsync() : await oq.ToListAsync()]
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> UpdateRubricsForArticle(ArticleRubricsSetModel req)
    {
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        if (req.RubricsIds.Length == 0)
            return new TResponseModel<bool>() { Response = await context.RubricsArticlesJoins.Where(x => x.ArticleId == req.ArticleId).ExecuteDeleteAsync() != 0 };

        RubricArticleJoinModelDB[] rubrics_db = await context
            .RubricsArticlesJoins
            .Where(x => x.ArticleId == req.ArticleId)
            .ToArrayAsync();

        bool res = false;
        int[] _ids = rubrics_db.Where(x => !req.RubricsIds.Contains(x.RubricId)).Select(x => x.Id).ToArray();
        if (_ids.Length != 0)
            res = await context.RubricsArticlesJoins.Where(x => _ids.Any(y => y == x.Id)).ExecuteDeleteAsync() != 0;

        _ids = req.RubricsIds.Where(x => !rubrics_db.Any(y => y.RubricId == x)).ToArray();
        if (_ids.Length != 0)
        {
            await context.AddRangeAsync(_ids.Select(x => new RubricArticleJoinModelDB() { ArticleId = req.ArticleId, RubricId = x }));
            res = res || await context.SaveChangesAsync() != 0;
        }

        return new()
        {
            Response = res
        };
    }
}