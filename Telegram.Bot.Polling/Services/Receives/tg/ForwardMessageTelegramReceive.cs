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
    : IResponseReceive<ForwardMessageTelegramBotModel?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ForwardTextMessageTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(ForwardMessageTelegramBotModel? message)
    {
        ArgumentNullException.ThrowIfNull(message);
        TResponseModel<int?> res = new() { Response = 0 };
        Message sender_msg;
        try
        {
            sender_msg = await _botClient.ForwardMessageAsync(chatId: message.DestinationChatId, fromChatId: message.SourceChatId, messageId: message.SourceMessageId);
            res.Response = sender_msg.MessageId;
            await storeTgRepo.StoreMessage(sender_msg);

        }
        catch (Exception ex)
        {
            TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
            await context.AddAsync(new ErrorSendingMessageTelegramBotModelDB() { ChatId = message.DestinationChatId, Message = ex.Message });
            await context.SaveChangesAsync();

            res.AddError("Ошибка отправки Telegram сообщения. error FA51C4EC-6AC7-4F7D-9B64-A6D6436DFDDA");

            res.Messages.InjectException(ex);
            return res;
        }

        return res;
    }
}