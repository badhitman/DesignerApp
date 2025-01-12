////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Services;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using RemoteCallLib;
using Telegram.Bot;
using SharedLib;
using DbcLib;
using Telegram.Bot.Exceptions;

namespace Transmission.Receives.telegram;

/// <summary>
/// Отправить сообщение пользователю через TelegramBot SendTextMessageTelegramBotModel
/// </summary>
public class SendTextMessageTelegramReceive(ITelegramBotClient _botClient,
    IDbContextFactory<TelegramBotContext> tgDbFactory,
    IWebTransmission webRemoteCall,
    StoreTelegramService storeTgRepo,
    ILogger<SendTextMessageTelegramReceive> _logger) : IResponseReceive<SendTextMessageTelegramBotModel?, TResponseModel<MessageComplexIdsModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SendTextMessageTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<MessageComplexIdsModel>?> ResponseHandleAction(SendTextMessageTelegramBotModel? message)
    {
        ArgumentNullException.ThrowIfNull(message);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(message)}");
        TResponseModel<MessageComplexIdsModel> res = new();
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

        IReplyMarkup? replyKB = message.ReplyKeyboard is null
            ? null
            : new InlineKeyboardMarkup(message.ReplyKeyboard
            .Select(x => x.Select(y => InlineKeyboardButton.WithCallbackData(y.Title, y.Data))));

        Message sender_msg;
        try
        {
            string msg_text = string.IsNullOrWhiteSpace(message.From)
                ? message.Message
                : $"{message.Message}\n--- {message.From.Trim()}";
            MessageTelegramModelDB msg_db;
            if (message.Files is not null && message.Files.Count != 0)
            {
                if (message.Files.Count == 1)
                {
                    FileAttachModel file = message.Files[0];

                    if (GlobalTools.IsImageFile(file.Name))
                    {
                        sender_msg = await _botClient.SendPhoto(
                                                            chatId: message.UserTelegramId,
                                                            photo: InputFile.FromStream(new MemoryStream(file.Data), file.Name),

                                                            caption: msg_text,
                                                            replyMarkup: replyKB,
                                                            parseMode: parse_mode,
                                                            replyParameters: message.ReplyToMessageId!.Value);
                    }
                    else
                    {
                        sender_msg = await _botClient.SendDocument(
                                    chatId: message.UserTelegramId,
                                    document: InputFile.FromStream(new MemoryStream(file.Data), file.Name),

                                    caption: msg_text,
                                    parseMode: parse_mode,
                                    replyParameters: message.ReplyToMessageId);
                    }

                    msg_db = await storeTgRepo.StoreMessage(sender_msg);
                    res.Response = new MessageComplexIdsModel()
                    {
                        DatabaseId = msg_db.Id,
                        TelegramId = sender_msg.MessageId
                    };
                }
                else
                {
                    Message[] senders_msgs = await _botClient.SendMediaGroup(
                        chatId: message.UserTelegramId,
                        media: message.Files.Select(ToolsStatic.ConvertFile).ToArray(),
                        replyParameters: message.ReplyToMessageId);

                    foreach (Message mm in senders_msgs)
                    {
                        msg_db = await storeTgRepo.StoreMessage(mm);
                        res.Response = new MessageComplexIdsModel()
                        {
                            DatabaseId = msg_db.Id,
                            TelegramId = mm.MessageId
                        };
                    }
                }
            }
            else
            {
                sender_msg = await _botClient.SendMessage(
                    chatId: message.UserTelegramId,
                    text: msg_text,
                    parseMode: parse_mode,
                    replyParameters: message.ReplyToMessageId);

                msg_db = await storeTgRepo.StoreMessage(sender_msg);
                res.Response = new MessageComplexIdsModel()
                {
                    DatabaseId = msg_db.Id,
                    TelegramId = sender_msg.MessageId
                };

            }
        }
        catch (Exception ex)
        {
            using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
            int? errorCode = null;
            if (ex is ApiRequestException _are)
                errorCode = _are.ErrorCode;
            else if (ex is RequestException _re)
                errorCode = (int?)_re.HttpStatusCode;

            await context.AddAsync(new ErrorSendingMessageTelegramBotModelDB()
            {
                ChatId = message.UserTelegramId,
                Message = $"{ex.Message}\n\n{JsonConvert.SerializeObject(message)}",
                ExceptionTypeName = ex.GetType().FullName,
                ErrorCode = errorCode
            });
            await context.SaveChangesAsync();

            msg = "Ошибка отправки Telegram сообщения. error FA51C4EC-6AC7-4F7D-9B64-A6D6436DFDDA";
            res.AddError(msg);
            _logger.LogError(ex, msg);
            res.Messages.InjectException(ex);
            return res;
        }

        if (message.MainTelegramMessageId.HasValue && message.MainTelegramMessageId != 0)
        {
            try
            {
                await _botClient.DeleteMessage(chatId: message.UserTelegramId, message.MainTelegramMessageId.Value);
            }
            finally { }
            await webRemoteCall.UpdateTelegramMainUserMessage(new() { MessageId = 0, UserId = message.UserTelegramId });
        }

        return res;
    }
}