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
    : IResponseReceive<GetIssuesForUserRequestModel?, TPaginationResponseModel<IssueHelpdeskModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssuesSelectHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModelDB>?>> ResponseHandleAction(GetIssuesForUserRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<TPaginationResponseModel<IssueHelpdeskModelDB>?> res = new()
        {
            Response = new()
        };

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IQueryable<IssueHelpdeskModelDB> q = context
            .Issues
            .Where(x => x.ProjectId == req.ProjectId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.SearchQuery))
        {
            req.SearchQuery = req.SearchQuery.ToUpper();
            
            q = from issue_element in q
                join rubric_element in context.RubricsForIssues on issue_element.RubricIssueId equals rubric_element.Id
                into grp_rubrics from c in grp_rubrics.DefaultIfEmpty()
                where issue_element.NormalizeNameUpper!.Contains(req.SearchQuery) || c.Name.Contains(req.SearchQuery)
                select issue_element;

        }

        switch (req.JournalMode)
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

        switch (req.UserArea)
        {
            case UsersAreasHelpdeskEnum.Author:
                q = q.Where(x => x.AuthorIdentityUserId == req.Request.IdentityUserId);
                break;
            case UsersAreasHelpdeskEnum.Subscriber:
                q = q.Where(x => context.SubscribersOfIssues.Any(y => y.IssueId == x.Id && y.AuthorIdentityUserId == req.Request.IdentityUserId));
                break;
            case UsersAreasHelpdeskEnum.Executor:
                q = q.Where(x => x.ExecutorIdentityUserId == req.Request.IdentityUserId);
                break;
            case UsersAreasHelpdeskEnum.Main:
                q = q.Where(x => x.ExecutorIdentityUserId == req.Request.IdentityUserId || context.SubscribersOfIssues.Any(y => y.IssueId == x.Id && y.AuthorIdentityUserId == req.Request.IdentityUserId));
                break;
        }
        res.Response.TotalRowsCount = await q.CountAsync();
        res.Response.Response = await q
            .Include(x => x.RubricIssue)
            .Include(x => x.Subscribers)
            .Include(x => x.Messages!)
            .ThenInclude(x => x.MarksAsResponse)
            .OrderBy(x => x.CreatedAt)
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize)
            .ToListAsync();

        return res;
    }
}