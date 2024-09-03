////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// IssuesForUserSelectReceive
/// </summary>
public class IssuesForUserSelectReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<TPaginationRequestModel<GetIssuesForUserRequestModel>?, TPaginationResponseModel<IssueHelpdeskModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssuesSelectHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>?>> ResponseHandleAction(TPaginationRequestModel<GetIssuesForUserRequestModel>? req)
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
            .Where(x => x.ProjectId == req.Payload.ProjectId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
        {
            req.Payload.SearchQuery = req.Payload.SearchQuery.ToUpper();

            q = from issue_element in q
                join rubric_element in context.RubricsForIssues on issue_element.RubricIssueId equals rubric_element.Id
                into grp_rubrics
                from c in grp_rubrics.DefaultIfEmpty()
                where issue_element.NormalizedNameUpper!.Contains(req.Payload.SearchQuery) || c.NormalizedNameUpper!.Contains(req.Payload.SearchQuery)
                select issue_element;
        }

        switch (req.Payload.JournalMode)
        {
            case HelpdeskJournalModesEnum.ActualOnly:
                q = q.Where(x => x.StepIssue <= HelpdeskIssueStepsEnum.Progress);
                break;
            case HelpdeskJournalModesEnum.ArchiveOnly:
                q = q.Where(x => x.StepIssue > HelpdeskIssueStepsEnum.Progress);
                break;
            default:
                break;
        }

        switch (req.Payload.UserArea)
        {
            case UsersAreasHelpdeskEnum.Subscriber:
                q = q.Where(x => context.SubscribersOfIssues.Any(y => y.IssueId == x.Id && req.Payload.IdentityUsersIds.Contains(y.UserId)));
                break;
            case UsersAreasHelpdeskEnum.Executor:
                q = q.Where(x => req.Payload.IdentityUsersIds.Contains(x.ExecutorIdentityUserId));
                break;
            case UsersAreasHelpdeskEnum.Main:
                q = q.Where(x => req.Payload.IdentityUsersIds.Contains(x.ExecutorIdentityUserId) || context.SubscribersOfIssues.Any(y => y.IssueId == x.Id && req.Payload.IdentityUsersIds.Contains(y.UserId)));
                break;
            case UsersAreasHelpdeskEnum.Author:
                q = q.Where(x => req.Payload.IdentityUsersIds.Contains(x.AuthorIdentityUserId));
                break;
            default:
                q = q.Where(x => req.Payload.IdentityUsersIds.Contains(x.AuthorIdentityUserId) || req.Payload.IdentityUsersIds.Contains(x.ExecutorIdentityUserId) || context.SubscribersOfIssues.Any(y => y.IssueId == x.Id && req.Payload.IdentityUsersIds.Contains(y.UserId)));
                break;
        }
        res.Response.TotalRowsCount = await q.CountAsync();

        var inc = q
            .OrderBy(x => x.CreatedAt)
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize)
            .Include(x => x.RubricIssue);

        List<IssueHelpdeskModelDB> data = req.Payload.IncludeSubscribers
            ? await inc.Include(x => x.Subscribers).ToListAsync()
            : await inc.ToListAsync();

        res.Response.Response = data.Select(x => IssueHelpdeskModel.Build(x)).ToList();

        return res;
    }
}