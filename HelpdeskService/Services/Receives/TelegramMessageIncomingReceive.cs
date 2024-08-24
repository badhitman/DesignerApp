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
    //ITelegramRemoteTransmissionService tgRepo,
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

        if (issues_for_user.Length == 1)
        {
            IssueHelpdeskModelDB issue_db = issues_for_user[0];

            StorageCloudParameterModel KeyStorage = new()
            {
                ApplicationName = GlobalStaticConstants.HelpdeskNotificationsTelegramAppName,
                Name = GlobalStaticConstants.Routes.ISSUE_CONTROLLER_NAME,
                OwnerPrimaryKey = issue_db.Id,
            };

            TResponseModel<long?> helpdesk_personal_redirect_telegram_for_issue_rest = await StorageRepo.ReadParameter<long?>(KeyStorage);
            if (!helpdesk_personal_redirect_telegram_for_issue_rest.Success() || !helpdesk_personal_redirect_telegram_for_issue_rest.Response.HasValue || helpdesk_personal_redirect_telegram_for_issue_rest.Response == 0)
            {
                res.AddRangeMessages(helpdesk_personal_redirect_telegram_for_issue_rest.Messages);
                res.AddError("Отсутствует значение helpdesk_personal_redirect_telegram_for_issue_rest");
                return res;
            }

            //TResponseModel<MessageComplexIdsModel?> forward_res = await tgRepo.ForwardMessage(new()
            //{
            //    DestinationChatId = helpdesk_personal_redirect_telegram_for_issue_rest.Response.Value,
            //    SourceChatId = req.Chat.ChatTelegramId,
            //    SourceMessageId = req.MessageTelegramId,
            //});
            //await context.AddAsync(new ForwardMessageTelegramBotModelDB()
            //{
            //    DestinationChatId = helpdesk_personal_redirect_telegram_for_issue_rest.Response.Value,
            //    ResultMessageId = forward_res.Response.DatabaseId,
            //    ResultMessageTelegramId = forward_res.Response.TelegramId,
            //     SourceChatId = req.Chat!.ChatTelegramId
            //});
        }

        return res;
    }
}