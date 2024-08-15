////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
///Subscribe update - of context user
/// </summary>
public class SubscribeUpdateReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory, IWebRemoteTransmissionService webTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<int>?, IssueHelpdeskModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SubscribeIssueUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB?>> ResponseHandleAction(TAuthRequestModel<int>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IssueHelpdeskModelDB? issue_db = await context
            .Issues
            .Include(x => x.RubricIssue)
            .Include(x => x.Messages)
            .Include(x => x.Subscribers)
            .FirstOrDefaultAsync(x => x.Id == req.Payload);

        if (issue_db is null)
        {
            return new()
            {
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Обращение не найдено или у вас нет к нему доступа" }]
            };
        }

        if (issue_db.ExecutorIdentityUserId == req.SenderActionUserId || issue_db.AuthorIdentityUserId == req.SenderActionUserId || issue_db.Subscribers!.Any(x => x.AuthorIdentityUserId == req.SenderActionUserId))
        {
            return new()
            {
                Response = issue_db
            };
        }

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.FindUsersIdentity([req.SenderActionUserId]);
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