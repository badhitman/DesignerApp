////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using Telegram.Bot;
using Telegram.Bot.Services;
using Telegram.Bot.Types.Enums;

namespace Transmission.Receives.telegram;

/// <summary>
/// Отправить сообщение пользователю через TelegramBot
/// </summary>
public class SendTextMessageTelegramReceive(ITelegramBotClient _botClient, IDbContextFactory<TelegramBotContext> tgDbFactory, IWebRemoteTransmissionService webRemoteCall, StoreTelegramService storeTgRepo, ILogger<SendTextMessageTelegramReceive> _logger)
    : IResponseReceive<SendTextMessageTelegramBotModel?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SendTextMessageTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(SendTextMessageTelegramBotModel? message)
    {
        ArgumentNullException.ThrowIfNull(message);
        TResponseModel<int?> res = new();
        string msg;
        if (string.IsNullOrWhiteSpace(message.Message))
        {
            res.AddError("Текст сообщения не может быть пустым");
            return res;
        }

        ParseMode parse_mode;
        if (Enum.TryParse(typeof(ParseMode), message.ParseModeName, true, out object? parse_mode_out))
            parse_mode = (ParseMode)parse_mode_out;
        else
        {
            parse_mode = ParseMode.Html;
            msg = $"Имя режима парсинга сообщения [{message.ParseModeName}] не допустимо. Установлен режим [{parse_mode}]. warning {{5A277B97-29B6-4B99-A022-A00E3F76E0C3}}";
            _logger.LogWarning(msg);
            res.AddWarning(msg);
        }

        Telegram.Bot.Types.Message sender_msg;
        try
        {
            string msg_text = string.IsNullOrWhiteSpace(message.From)
                ? ""
                : $"{message.Message}\n--- {message.From.Trim()}";

            sender_msg = await _botClient.SendTextMessageAsync(
                chatId: message.UserTelegram.TelegramId,
                text: msg_text,
                parseMode: parse_mode,
                replyToMessageId: message.ReplyToMessageId);

            await storeTgRepo.StoreMessage(sender_msg);
#if DEBUG
            await _botClient.EditMessageTextAsync(
                chatId: message.UserTelegram.TelegramId,
                messageId: sender_msg.MessageId,
                text: $"#<b>{sender_msg.MessageId}</b>\n{msg_text}",
                parseMode: ParseMode.Html
                );
#endif
        }
        catch (Exception ex)
        {
            TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
            await context.AddAsync(new ErrorSendingTextMessageTelegramBotModelDB() { Name = ex.GetType().FullName ?? "err", ChatId = message.UserTelegram.TelegramId, Tag = ex.Message });
            await context.SaveChangesAsync();

            msg = "Ошибка отправки Telegram сообщения. error FA51C4EC-6AC7-4F7D-9B64-A6D6436DFDDA";
            res.AddError(msg);
            _logger.LogError(ex, msg);
            res.Messages.InjectException(ex);
            return res;
        }
        res.Response = sender_msg.MessageId;
        if (message.UserTelegram.MainTelegramMessageId.HasValue && message.UserTelegram.MainTelegramMessageId != 0)
        {
            try
            {
                await _botClient.DeleteMessageAsync(chatId: message.UserTelegram.TelegramId, message.UserTelegram.MainTelegramMessageId.Value);
            }
            finally { }
            await webRemoteCall.UpdateTelegramMainUserMessage(new() { MessageId = 0, UserId = message.UserTelegram.TelegramId });
        }

        return res;
    }
}