////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// TelegramMessageIncomingReceive
/// </summary>
public class TelegramMessageIncomingReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    ITelegramRemoteTransmissionService tgRepo,
    ISerializeStorageRemoteTransmissionService StorageRepo)
    : IResponseReceive<TelegramIncomingMessageModel?, bool>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IncomingTelegramMessageHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ResponseHandleAction(TelegramIncomingMessageModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool> res = new() { Response = false };
        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IssueHelpdeskModelDB[] issues_for_user = await context
            .Issues
            .Where(x => x.AuthorIdentityUserId == req.User.UserIdentityId)
            .ToArrayAsync();

        //ForwardMessageTelegramBotModelDB
        if (issues_for_user.Length == 1)
        {
            IssueHelpdeskModelDB issue_db = issues_for_user[0];

            StorageCloudParameterModel KeyStorage = new()
            {
                ApplicationName = GlobalStaticConstants.HelpdeskNotificationsTelegramAppName,
                Name = GlobalStaticConstants.Routes.ISSUE_CONTROLLER_NAME,
                OwnerPrimaryKey = issue_db.Id,
            };
            TResponseModel<long?> rest = await StorageRepo.ReadParameter<long?>(KeyStorage);
            if (rest.Response.HasValue && rest.Response != 0)
            {
                //var v = await tgRepo.
            }
        }

        return res;
    }
}