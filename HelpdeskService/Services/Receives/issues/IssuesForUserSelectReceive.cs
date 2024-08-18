////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// GetIssuesForUser
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

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

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
                where issue_element.NormalizeNameUpper!.Contains(req.Payload.SearchQuery) || c.NormalizedNameToUpper!.Contains(req.Payload.SearchQuery)
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
                q = q.Where(x => context.SubscribersOfIssues.Any(y => y.IssueId == x.Id && y.UserId == req.Payload.IdentityUserId));
                break;
            case UsersAreasHelpdeskEnum.Executor:
                q = q.Where(x => x.ExecutorIdentityUserId == req.Payload.IdentityUserId);
                break;
            case UsersAreasHelpdeskEnum.Main:
                q = q.Where(x => x.ExecutorIdentityUserId == req.Payload.IdentityUserId || context.SubscribersOfIssues.Any(y => y.IssueId == x.Id && y.UserId == req.Payload.IdentityUserId));
                break;
            default:
                q = q.Where(x => x.AuthorIdentityUserId == req.Payload.IdentityUserId);
                break;
        }
        res.Response.TotalRowsCount = await q.CountAsync();

        List<IssueHelpdeskModelDB> data = await q
            .Include(x => x.RubricIssue)
            .OrderBy(x => x.CreatedAt)
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize)
            .ToListAsync();

        res.Response.Response = data.Select(x => IssueHelpdeskModel.Build(x)).ToList();

        return res;
    }
}