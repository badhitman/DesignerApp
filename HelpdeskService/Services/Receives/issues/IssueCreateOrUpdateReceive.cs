////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Create (or update) Issue: Рубрика, тема и описание
/// </summary>
public class IssueCreateOrUpdateReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory, IWebRemoteTransmissionService webTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<IssueUpdateRequestModel>?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssueUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(TAuthRequestModel<IssueUpdateRequestModel>? issue_upd)
    {
        ArgumentNullException.ThrowIfNull(issue_upd);
        TResponseModel<int?> res = new();

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.FindUsersIdentity([issue_upd.SenderActionUserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

        issue_upd.Payload.Description = issue_upd.Payload.Description?.Trim();
        issue_upd.Payload.Name = issue_upd.Payload.Name.Trim();
        string normalizeNameUpper = issue_upd.Payload.Name.ToUpper();
        IssueHelpdeskModelDB issue;

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        if (issue_upd.Payload.Id < 1)
        {
            issue = new()
            {
                AuthorIdentityUserId = issue_upd.SenderActionUserId,
                Name = issue_upd.Payload.Name,
                Description = issue_upd.Payload.Description,
                RubricIssueId = issue_upd.Payload.RubricIssueId,
                NormalizeNameUpper = normalizeNameUpper,
                StepIssue = HelpdeskIssueStepsEnum.Created,
                ProjectId = issue_upd.Payload.ProjectId,
            };

            await context.AddAsync(issue);
            await context.SaveChangesAsync();
            issue_upd.Payload.Id = issue.Id;
            res.AddSuccess("Обращение успешно создано");
            res.Response = issue_upd.Payload.Id;
        }
        else
        {
            IssueHelpdeskModelDB issue_db = await context
                .Issues
                .Include(x => x.Subscribers)
                .FirstAsync(x => x.Id == issue_upd.Payload.Id);

            if (issue_db.AuthorIdentityUserId == issue_upd.SenderActionUserId ||
                issue_db.ExecutorIdentityUserId == issue_upd.SenderActionUserId ||
                issue_db.Subscribers?.Any(x => x.UserId == issue_upd.SenderActionUserId) == true ||
                actor.Roles?.Any(x => x.Equals(GlobalStaticConstants.Roles.HelpDeskTelegramBotManager) || x.Equals(GlobalStaticConstants.Roles.HelpDeskTelegramBotUnit)) == true ||
                actor.IsAdmin)
            {
                res.Response = await context.Issues.Where(x => x.Id == issue_upd.Payload.Id)
                                .ExecuteUpdateAsync(setters => setters
                                .SetProperty(b => b.Description, issue_upd.Payload.Description)
                                .SetProperty(b => b.NormalizeNameUpper, normalizeNameUpper)
                                .SetProperty(b => b.RubricIssueId, issue_upd.Payload.RubricIssueId)
                                .SetProperty(b => b.Name, issue_upd.Payload.Name)
                                .SetProperty(b => b.LastUpdateAt, DateTime.UtcNow));

                res.AddSuccess("Обращение успешно обновлено");
            }
            else
                res.AddError($"У вас не достаточно прав для редактирования этого обращения #{issue_upd.Payload.Id} '{issue_db.Name}'");
        }
        

        return res;
    }
}