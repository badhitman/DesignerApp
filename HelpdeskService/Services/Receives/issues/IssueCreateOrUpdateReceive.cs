////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;
using Microsoft.EntityFrameworkCore.Query;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Create (or update) Issue: Рубрика, тема и описание
/// </summary>
public class IssueCreateOrUpdateReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory, IWebRemoteTransmissionService webTransmissionRepo)
    : IResponseReceive<IssueUpdateRequest?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssueUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(IssueUpdateRequest? issue_upd)
    {
        ArgumentNullException.ThrowIfNull(issue_upd);
        TResponseModel<int?> res = new();

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.FindUsersIdentity([issue_upd.ActionUserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

        issue_upd.Description = issue_upd.Description?.Trim();
        issue_upd.Name = issue_upd.Name.Trim();
        string normalizeNameUpper = issue_upd.Name.ToUpper();
        IssueHelpdeskModelDB issue;

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        if (issue_upd.Id < 1)
        {
            issue = new()
            {
                AuthorIdentityUserId = issue_upd.ActionUserId,
                Name = issue_upd.Name,
                Description = issue_upd.Description,
                RubricIssueId = issue_upd.RubricIssueId,
                NormalizeNameUpper = normalizeNameUpper,
                StepIssue = HelpdeskIssueStepsEnum.Created,
                ProjectId = issue_upd.ProjectId,
            };

            await context.AddAsync(issue);
            await context.SaveChangesAsync();
            issue_upd.Id = issue.Id;
            res.AddSuccess("Обращение успешно создано");
        }
        else
        {
            IssueHelpdeskModelDB issue_db = await context
                .Issues
                .Include(x => x.Subscribers)
                .FirstAsync(x => x.Id == issue_upd.Id);

            if (issue_db.AuthorIdentityUserId == issue_upd.ActionUserId ||
                issue_db.ExecutorIdentityUserId == issue_upd.ActionUserId ||
                issue_db.Subscribers?.Any(x => x.AuthorIdentityUserId == issue_upd.ActionUserId) == true ||
                actor.Roles?.Any(x => x.Equals(GlobalStaticConstants.Roles.HelpDeskTelegramBotManager) || x.Equals(GlobalStaticConstants.Roles.HelpDeskTelegramBotUnit)) == true ||
                actor.IsAdmin)
            {
                await context.Issues.Where(x => x.Id == issue_upd.Id)
                                .ExecuteUpdateAsync(setters => setters
                                .SetProperty(b => b.Description, issue_upd.Description)
                                .SetProperty(b => b.NormalizeNameUpper, normalizeNameUpper)
                                .SetProperty(b => b.RubricIssueId, issue_upd.RubricIssueId)
                                .SetProperty(b => b.Name, issue_upd.Name)
                                .SetProperty(b => b.LastUpdateAt, DateTime.UtcNow));

                res.AddSuccess("Обращение успешно обновлено");
            }
            else
                res.AddError($"У вас не достаточно прав для редактирования этого обращения #{issue_upd.Id} '{issue_db.Name}'");
        }
        res.Response = issue_upd.Id;

        return res;
    }
}