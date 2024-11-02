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
public class ConsoleIssuesSelectReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory, IManualCustomCacheService cacheRepo)
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

        string cst = Path.Combine(GlobalStaticConstants.Routes.CONSOLE_CONTROLLER_NAME, req.Payload.Status.ToString());
        MemCacheComplexKeyModel mceKey = new(GlobalStaticConstants.Routes.DEFAULT_CONTROLLER_NAME, new MemCachePrefixModel(cst, GlobalStaticConstants.Routes.SELECT_ACTION_NAME));
        string cacheToken = await cacheRepo.GetStringValueAsync(mceKey) ?? NewToken;
        
        TPaginationResponseModel<IssueHelpdeskModel>? _fr = await cacheRepo.GetObjectAsync<TPaginationResponseModel<IssueHelpdeskModel>>(cacheToken);
        if (_fr is not null)
            return new() { Response = _fr };

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

        await cacheRepo.SetObject(new MemCacheComplexKeyModel(cacheToken, new MemCachePrefixModel(cst, QueueName)), res);

        return new() { Response = res };
    }

    static string NewToken => $"{DateTime.UtcNow.Millisecond}/{Guid.NewGuid}";
}