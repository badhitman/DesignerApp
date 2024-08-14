////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Read issue - of context user
/// </summary>
public class IssueReadReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory, IWebRemoteTransmissionService webTransmissionRepo)
    : IResponseReceive<IssueReadRequestModel?, IssueHelpdeskModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssueGetHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB?>> ResponseHandleAction(IssueReadRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IssueHelpdeskModelDB? issue_db = await context
            .Issues
            .Include(x => x.RubricIssue)
            .Include(x => x.Messages)
            .Include(x => x.Subscribers)
            .FirstOrDefaultAsync(x => x.Id == req.IssueId);

        if (issue_db is null)
        {
            return new()
            {
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Обращение не найдено или у вас нет к нему доступа" }]
            };
        }

        if (issue_db.ExecutorIdentityUserId == req.UserIdentityId || issue_db.AuthorIdentityUserId == req.UserIdentityId || issue_db.Subscribers!.Any(x => x.AuthorIdentityUserId == req.UserIdentityId))
        {
            return new()
            {
                Response = issue_db
            };
        }

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.FindUsersIdentity([req.UserIdentityId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };


        string[] job = [GlobalStaticConstants.Roles.HelpDeskTelegramBotManager, GlobalStaticConstants.Roles.HelpDeskTelegramBotUnit, GlobalStaticConstants.Roles.Admin];
        if (rest.Response[0].Roles?.Any(x => job.Contains(x)) != true)
        {
            return new()
            {
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Обращение не найдено или у вас нет к нему доступа" }]
            };
        }

        return new()
        {
            Response = issue_db
        };
    }
}