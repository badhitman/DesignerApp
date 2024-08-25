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

        if (req.ReplyToMessage is not null)
        {
            ForwardMessageTelegramBotModelDB? inc_msg = await context.ForwardedMessages
            .FirstOrDefaultAsync(x =>
            x.DestinationChatId == req.ReplyToMessage.Chat!.ChatTelegramId &&
            x.SourceChatId == req.ReplyToMessage.ForwardFromId &&
            x.ResultMessageTelegramId == req.ReplyToMessage.MessageTelegramId);

            // AnswerToForwardModelDB

            if (inc_msg is not null)
            {
                SendTextMessageTelegramBotModel sender = new()
                {
                    Message = req.Text ?? req.Caption ?? "Вложения",
                    UserTelegramId = req.ReplyToMessage.Chat!.ChatTelegramId,

                };

                TResponseModel<MessageComplexIdsModel?> send_answer = await tgRepo.SendTextMessageTelegram(sender);

                res.AddInfo("Данное сообщение является ответом на другое сообщение!");
                res.Response = true;
                return res;
            }
        }

        StorageCloudParameterModel key_storage = new()
        {
            ApplicationName = GlobalStaticConstants.HelpdeskNotificationsTelegramAppName,
            Name = GlobalStaticConstants.Routes.USER_CONTROLLER_NAME,
            PrefixPropertyName = req.User.UserIdentityId,
        };

        TResponseModel<long?> helpdesk_user_redirect_telegram_for_issue_rest = await StorageRepo.ReadParameter<long?>(key_storage);
        if (helpdesk_user_redirect_telegram_for_issue_rest.Success() && helpdesk_user_redirect_telegram_for_issue_rest.Response.HasValue && helpdesk_user_redirect_telegram_for_issue_rest.Response != 0)
        {
            TResponseModel<MessageComplexIdsModel?> forward_res = await tgRepo.ForwardMessage(new()
            {
                DestinationChatId = helpdesk_user_redirect_telegram_for_issue_rest.Response.Value,
                SourceChatId = req.Chat!.ChatTelegramId,
                SourceMessageId = req.MessageTelegramId,
            });

            if (forward_res.Success() && forward_res.Response is not null)
            {
                await context.AddAsync(new ForwardMessageTelegramBotModelDB()
                {
                    DestinationChatId = helpdesk_user_redirect_telegram_for_issue_rest.Response.Value,
                    ResultMessageId = forward_res.Response.DatabaseId,
                    ResultMessageTelegramId = forward_res.Response.TelegramId,
                    SourceChatId = req.Chat!.ChatTelegramId,
                    SourceMessageId = req.MessageTelegramId,
                });
                await context.SaveChangesAsync();
                res.AddSuccess($"Сообщение было переслано в чат #{helpdesk_user_redirect_telegram_for_issue_rest.Response.Value} по подписке на пользователя");
                return res;
            }
            else
                res.AddRangeMessages(forward_res.Messages);
        }

        key_storage = new()
        {
            ApplicationName = GlobalStaticConstants.Routes.HELPDESK_CONTROLLER_NAME,
            Name = $"{GlobalStaticConstants.Routes.TELEGRAM_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.NOTIFICATIONS_CONTROLLER_NAME}",
            PrefixPropertyName = GlobalStaticConstants.Routes.GLOBAL_CONTROLLER_NAME,
        };

        helpdesk_user_redirect_telegram_for_issue_rest = await StorageRepo.ReadParameter<long?>(key_storage);
        if (helpdesk_user_redirect_telegram_for_issue_rest.Success() && helpdesk_user_redirect_telegram_for_issue_rest.Response.HasValue && helpdesk_user_redirect_telegram_for_issue_rest.Response != 0)
        {
            TResponseModel<MessageComplexIdsModel?> forward_res = await tgRepo.ForwardMessage(new()
            {
                DestinationChatId = helpdesk_user_redirect_telegram_for_issue_rest.Response.Value,
                SourceChatId = req.Chat!.ChatTelegramId,
                SourceMessageId = req.MessageTelegramId,
            });

            if (forward_res.Success() && forward_res.Response is not null)
            {
                await context.AddAsync(new ForwardMessageTelegramBotModelDB()
                {
                    DestinationChatId = helpdesk_user_redirect_telegram_for_issue_rest.Response.Value,
                    ResultMessageId = forward_res.Response.DatabaseId,
                    ResultMessageTelegramId = forward_res.Response.TelegramId,
                    SourceChatId = req.Chat!.ChatTelegramId,
                    SourceMessageId = req.MessageTelegramId,
                });
                await context.SaveChangesAsync();
                res.AddSuccess($"Сообщение было переслано в чат #{helpdesk_user_redirect_telegram_for_issue_rest.Response.Value} по глобальной подписке");
                return res;
            }
            else
                res.AddRangeMessages(forward_res.Messages);
        }

        return res;
    }
}