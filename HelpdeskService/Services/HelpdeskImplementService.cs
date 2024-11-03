////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLib;
using DbcLib;

namespace HelpdeskService;

/// <summary>
/// Helpdesk - Implement
/// </summary>
public class HelpdeskImplementService(IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IManualCustomCacheService cacheRepo, IOptions<HelpdeskConfigModel> hdConf) : IHelpdeskService
{
    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<IssueHelpdeskModel>> ConsoleIssuesSelect(TPaginationRequestModel<ConsoleIssuesRequestModel> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        static string ConsoleSegmentNewCacheToken(StatusesDocumentsEnum st) => $"{GlobalStaticConstants.Routes.CONSOLE_CONTROLLER_NAME}:{GlobalStaticConstants.Routes.SEGMENT_CONTROLLER_NAME}:{st}:{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.{Guid.NewGuid()}";
        MemCacheComplexKeyModel mceKey = GlobalStaticConstants.Cache.ConsoleSegmentStatusToken(req.Payload.Status);
        string? cacheToken = await cacheRepo.GetStringValueAsync(mceKey);
        if (string.IsNullOrWhiteSpace(cacheToken))
        {
            cacheToken = ConsoleSegmentNewCacheToken(req.Payload.Status);
            await cacheRepo.SetStringAsync(mceKey, cacheToken);
        }

        TPaginationResponseModel<IssueHelpdeskModel>? _fr = await cacheRepo.GetObjectAsync<TPaginationResponseModel<IssueHelpdeskModel>>(cacheToken);
        if (_fr is not null)
            return _fr;

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        IQueryable<IssueHelpdeskModelDB> q = context
            .Issues
            .Where(x => x.ProjectId == req.Payload.ProjectId && x.StepIssue == req.Payload.Status)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
        {
            req.Payload.SearchQuery = req.Payload.SearchQuery.ToUpper();
            q = from issue_element in q
                join rubric_element in context.Rubrics on issue_element.RubricIssueId equals rubric_element.Id into outer_rubric
                from rubric_item in outer_rubric.DefaultIfEmpty()
                where issue_element.NormalizedNameUpper!.Contains(req.Payload.SearchQuery) || issue_element.NormalizedDescriptionUpper!.Contains(req.Payload.SearchQuery) || rubric_item.NormalizedNameUpper!.Contains(req.Payload.SearchQuery)
                select issue_element;
        }

        if (!string.IsNullOrWhiteSpace(req.Payload.FilterUserId))
            q = q.Where(x => x.AuthorIdentityUserId == req.Payload.FilterUserId || x.ExecutorIdentityUserId == req.Payload.FilterUserId || context.SubscribersOfIssues.Any(y => y.IssueId == x.Id && y.UserId == req.Payload.FilterUserId));

        IOrderedQueryable<IssueHelpdeskModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
            ? q.OrderBy(x => x.CreatedAt)
            : q.OrderByDescending(x => x.CreatedAt);

        List<IssueHelpdeskModelDB> data = await oq
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize)
            .Include(x => x.RubricIssue)
            .ToListAsync();

        TPaginationResponseModel<IssueHelpdeskModel> res = new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortingDirection = req.SortingDirection,
            SortBy = req.SortBy,
            TotalRowsCount = await q.CountAsync(),
            Response = [.. data.Select(x => IssueHelpdeskModel.Build(x))]
        };

        await cacheRepo.SetObject(cacheToken, res, TimeSpan.FromSeconds(hdConf.Value.ConsoleSegmentCacheLifetimeSeconds));
        return res;
    }


}