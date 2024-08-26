////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Services;
using Telegram.Bot.Types;
using RemoteCallLib;
using Telegram.Bot;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Переслать сообщение пользователю через TelegramBot ForwardMessageTelegramReceive
/// </summary>
public class ForwardMessageTelegramReceive(
    ITelegramBotClient _botClient,
    IDbContextFactory<TelegramBotContext> tgDbFactory,
    StoreTelegramService storeTgRepo)
    : IResponseReceive<ForwardMessageTelegramBotModel?, MessageComplexIdsModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ForwardTextMessageTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<MessageComplexIdsModel?>> ResponseHandleAction(ForwardMessageTelegramBotModel? message)
    {
        ArgumentNullException.ThrowIfNull(message);
        TResponseModel<MessageComplexIdsModel?> res = new();
        Message sender_msg;
        try
        {
            sender_msg = await _botClient.ForwardMessageAsync(chatId: message.DestinationChatId, fromChatId: message.SourceChatId, messageId: message.SourceMessageId);

            MessageTelegramModelDB msg_db = await storeTgRepo.StoreMessage(sender_msg);
            res.Response = new()
            {
                TelegramId = sender_msg.MessageId,
                DatabaseId = msg_db.Id,
            };
        }
        catch (Exception ex)
        {
            TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
            await context.AddAsync(new ErrorSendingMessageTelegramBotModelDB()
            {
                ChatId = message.DestinationChatId,
                Message = ex.Message,
                ExceptionTypeName = ex.GetType().FullName,
            });
            await context.SaveChangesAsync();

            res.AddError("Ошибка отправки Telegram сообщения. error E06E939D-6E93-45CE-A5F5-19A417A27DC1");

            res.Messages.InjectException(ex);
            return res;
        }

        return res;
    }
}