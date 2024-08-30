using ServerLib;
using SharedLib;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Services;

/// <summary>
/// Processes <see cref="Update"/>s and errors.
/// <para>See <see cref="DefaultUpdateHandler"/> for a simple implementation</para>
/// </summary>
public class UpdateHandler(
    StoreTelegramService storeRepo,
    ITelegramBotClient botClient,
    ILogger<UpdateHandler> logger,
    ISerializeStorageRemoteTransmissionService serializeStorageRepo,
    IWebRemoteTransmissionService webRemoteRepo,
    IHelpdeskRemoteTransmissionService helpdeskRepo,
    IServiceProvider servicesProvider) : IUpdateHandler
{
    static readonly Type defHandlerType = typeof(DefaultTelegramDialogHandle);

    /// <inheritdoc/>
    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        Task handler = update switch
        {
            // UpdateType.Unknown:
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            // UpdateType.Poll:
            //{ ChosenInlineResult: { } chosenInlineResult } => BotOnChosenInlineResultReceived(chosenInlineResult, cancellationToken),
            //{ InlineQuery: { } inlineQuery } => BotOnInlineQueryReceived(inlineQuery, cancellationToken),
            //{ EditedMessage: { } message } => BotOnMessageReceived(message, cancellationToken),
            { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
            { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
            _ => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Receive message type: {MessageType}", message.Type);
        await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken);

        MessageTelegramModelDB msg_db = await storeRepo.StoreMessage(message);

        string messageText = message.Text?.Trim() ?? message.Caption?.Trim() ?? "Вложения";
        if (message.From is null || (string.IsNullOrWhiteSpace(messageText) && string.IsNullOrEmpty(message.Caption) && message.Audio is null && message.Contact is null && message.Document is null && message.Photo is null && message.Video is null && message.Voice is null))
            return;

        TResponseModel<CheckTelegramUserAuthModel?> uc = await webRemoteRepo.CheckTelegramUser(CheckTelegramUserHandleModel.Build(message.From.Id, message.From.FirstName, message.From.LastName, message.From.Username, message.From.IsBot));

        if (uc.Response is not null)
        {
            TelegramIncomingMessageModel hd_request = new()
            {
                Audio = msg_db.Audio,
                User = uc.Response,
                Voice = msg_db.Voice,
                Video = msg_db.Video,
                AudioId = msg_db.AudioId,
                AuthorSignature = msg_db.AuthorSignature,
                Caption = msg_db.Caption,
                Chat = msg_db.Chat,
                ChatId = msg_db.ChatId,
                Contact = msg_db.Contact,
                ContactId = msg_db.ContactId,
                CreatedAtUtc = msg_db.CreatedAtUtc,
                DocumentId = msg_db.DocumentId,
                ForwardDate = msg_db.ForwardDate,
                ForwardFromChatId = msg_db.ForwardFromChatId,
                EditDate = msg_db.EditDate,
                ForwardFromId = msg_db.ForwardFromId,
                ForwardFromMessageId = msg_db.ForwardFromMessageId,
                ForwardSenderName = msg_db.ForwardSenderName,
                ForwardSignature = msg_db.ForwardSignature,
                From = msg_db.From,
                FromId = msg_db.FromId,
                Id = msg_db.Id,
                Document = msg_db.Document,
                IsAutomaticForward = msg_db.IsAutomaticForward,
                IsTopicMessage = msg_db.IsTopicMessage,
                MediaGroupId = msg_db.MediaGroupId,
                MessageTelegramId = msg_db.MessageTelegramId,
                MessageThreadId = msg_db.MessageThreadId,
                NormalizedCaptionUpper = msg_db.NormalizedCaptionUpper,
                NormalizedTextUpper = msg_db.NormalizedTextUpper,
                ReplyToMessageId = msg_db.ReplyToMessageId,
                SenderChatId = msg_db.SenderChatId,
                Text = msg_db.Text,
                Photo = msg_db.Photo,
                ViaBotId = msg_db.ViaBotId,
                VideoId = msg_db.VideoId,
                VoiceId = msg_db.VoiceId,
                ReplyToMessage = msg_db.ReplyToMessage,
            };

            TResponseModel<bool?> hd_res = await helpdeskRepo.TelegramMessageIncoming(hd_request);

            if (message.Chat.Type == ChatType.Private)
                await Usage(uc.Response, message.MessageId, MessagesTypesEnum.TextMessage, message.Chat.Id, messageText, cancellationToken);
        }
    }

    // Process Inline Keyboard callback data
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);

        if (callbackQuery.Message?.From is null || string.IsNullOrEmpty(callbackQuery.Data))
            return;

        await botClient.SendChatActionAsync(callbackQuery.Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken);
        TResponseModel<CheckTelegramUserAuthModel?> uc = await webRemoteRepo.CheckTelegramUser(CheckTelegramUserHandleModel.Build(callbackQuery.From.Id, callbackQuery.From.FirstName, callbackQuery.From.LastName, callbackQuery.From.Username, callbackQuery.From.IsBot));
        await Usage(uc.Response!, callbackQuery.Message.MessageId, MessagesTypesEnum.CallbackQuery, callbackQuery.Message.Chat.Id, callbackQuery.Data, cancellationToken);
    }

    private async Task Usage(TelegramUserBaseModel uc, int incomingMessageId, MessagesTypesEnum eventType, ChatId chatId, string messageText, CancellationToken cancellationToken)
    {
        string msg;
        uc.DialogTelegramTypeHandler ??= typeof(DefaultTelegramDialogHandle).FullName;
        if (eventType == MessagesTypesEnum.TextMessage)
        {
            ResponseBaseModel? check_token = null;
            if (messageText.StartsWith("/start ") && Guid.TryParse(messageText[7..], out _))
            {
                messageText = messageText[7..];
                check_token = await webRemoteRepo.TelegramJoinAccountConfirmToken(new() { TelegramUser = uc, Token = messageText.Trim() });
            }
            else if (Guid.TryParse(messageText.Trim(), out _))
                check_token = await webRemoteRepo.TelegramJoinAccountConfirmToken(new() { TelegramUser = uc, Token = messageText.Trim() });

            if (check_token is not null)
            {
                if (!check_token.Success())
                {
                    msg = $"Ошибка проверки токена: {check_token.Message()}. Начать заново /start";
                    Message msg_s = await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: msg,
                                    parseMode: ParseMode.Html,
                                    replyMarkup: new ReplyKeyboardRemove(),
                                    cancellationToken: cancellationToken);
#if DEBUG
                    await botClient.EditMessageTextAsync(
                        chatId: msg_s.Chat.Id,
                        messageId: msg_s.MessageId,
                        text: $"#<b>{msg_s.MessageId}</b>\n{msg}",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
#endif
                }
                else
                {
                    msg = $"Токен успешно проверен";
                    Message msg_s = await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: msg,
                                    parseMode: ParseMode.Html,
                                    replyMarkup: new ReplyKeyboardRemove(),
                                    cancellationToken: cancellationToken);

#if DEBUG
                    await botClient.EditMessageTextAsync(
                        chatId: msg_s.Chat.Id,
                        messageId: msg_s.MessageId,
                        text: $"#<b>{msg_s.MessageId}</b>\n{msg}",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
#endif

                }

                return;
            }
        }

        TResponseModel<bool?> res_IsCommandModeTelegramBot = await serializeStorageRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ParameterIsCommandModeTelegramBot);

        if (res_IsCommandModeTelegramBot.Response == true && !messageText.StartsWith('/') && eventType != MessagesTypesEnum.CallbackQuery)
            return;

        ITelegramDialogService? receiveService;
        using IServiceScope scope = servicesProvider.CreateScope();
        receiveService = scope.ServiceProvider
            .GetServices<ITelegramDialogService>()
            .FirstOrDefault(o => o.GetType().FullName == uc.DialogTelegramTypeHandler);

        if (receiveService is null)
        {
            if (!string.IsNullOrWhiteSpace(uc.DialogTelegramTypeHandler))
                logger.LogError($"Ошибка в имени {nameof(uc.DialogTelegramTypeHandler)}: {uc.DialogTelegramTypeHandler}. error {{2A878102-BC1A-4637-8B02-D33DCE1E7591}}");

            receiveService = scope.ServiceProvider
                .GetServices<ITelegramDialogService>()
            .First(o => o.GetType() == defHandlerType);
        }

        TelegramDialogResponseModel resp = await receiveService.TelegramDialogHandle(new TelegramDialogRequestModel()
        {
            MessageText = messageText,
            MessageTelegramId = incomingMessageId,
            TelegramUser = uc,
            TypeMessage = eventType,
        });

        if (!resp.Success())
        {
            msg = $"Ошибка обработки ответа на входящее сообщение Telegram: {resp.Message()}. error {{3A3ABECF-6CFB-4FF5-AE63-A124308C5EE8}}";
            logger.LogError(msg);
            Message msg_s = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: msg,
                                parseMode: ParseMode.Html,
                                replyMarkup: new ReplyKeyboardRemove(),
                                cancellationToken: cancellationToken);

#if DEBUG
            await botClient.EditMessageTextAsync(
                chatId: msg_s.Chat.Id,
                messageId: msg_s.MessageId,
                text: $"#<b>{msg_s.MessageId}</b>\n{msg}",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
#endif

            return;
        }

        IReplyMarkup? replyKB = resp.ReplyKeyboard is null
            ? null
            : new InlineKeyboardMarkup(resp.ReplyKeyboard
            .Select(x => x.Select(y => InlineKeyboardButton.WithCallbackData(y.Title, y.Data))));

        ResponseBaseModel upd_main_msg_res;
        Message? messageSended = null;
        if (resp.ReplyKeyboard is null && string.IsNullOrEmpty(resp.Response))
        {
            msg = $"Ошибка обработки ответа на входящее сообщение Telegram: [ReplyKeyboard && ResponseText] is null. error {{A3A9DF60-8BC9-4868-8BD8-F29B64AE39CD}}";
            logger.LogError(msg);
#if DEBUG            
            messageSended = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: msg,
                                parseMode: ParseMode.Html,
                                replyMarkup: new ReplyKeyboardRemove(),
                                cancellationToken: cancellationToken);

            await botClient.EditMessageTextAsync(
                chatId: messageSended.Chat.Id,
                messageId: messageSended.MessageId,
                text: $"#<b>{messageSended.MessageId}</b>\n{msg}",
                parseMode: ParseMode.Html,
                replyMarkup: (InlineKeyboardMarkup?)replyKB,
                cancellationToken: cancellationToken);
#endif

            return;
        }
        else if (string.IsNullOrEmpty(resp.Response))
        {
            if (uc.MainTelegramMessageId.HasValue)
            {
                try
                {
                    messageSended = await botClient.EditMessageReplyMarkupAsync(chatId: chatId, messageId: uc.MainTelegramMessageId.Value, replyMarkup: (InlineKeyboardMarkup?)replyKB, cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    msg = $"Привет, {uc}!";
                    logger.LogError(ex, $"{msg} error {{F0A92AB9-9136-43C0-9422-AADA61A82841}}");
                    messageSended = await botClient.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: msg,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: replyKB,
                                        cancellationToken: cancellationToken);
                    upd_main_msg_res = await webRemoteRepo
                    .UpdateTelegramMainUserMessage(new() { MessageId = messageSended.MessageId, UserId = uc.TelegramId });

                    if (!upd_main_msg_res.Success())
                        logger.LogError($"Попытка установить TelegramMessageId #{messageSended.MessageId} для [основного сообщения] {uc} вернула ошибку:\n{upd_main_msg_res.Message()}");
                    else
                        logger.LogDebug($"TelegramMessageId для [основного сообщения] {uc} изменён на #{messageSended.MessageId}.");
#if DEBUG
                    await botClient.EditMessageTextAsync(
                        chatId: messageSended.Chat.Id,
                        messageId: messageSended.MessageId,
                        text: $"#<b>{messageSended.MessageId}</b>\n{msg}",
                        parseMode: ParseMode.Html,
                        replyMarkup: (InlineKeyboardMarkup?)replyKB,
                        cancellationToken: cancellationToken);
#endif
                }
            }
            else
            {
                msg = $"Привет, {uc}!";
                messageSended = await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: msg,
                                    parseMode: ParseMode.Html,
                                    replyMarkup: replyKB,
                                    cancellationToken: cancellationToken);
                upd_main_msg_res = await webRemoteRepo
                .UpdateTelegramMainUserMessage(new() { MessageId = messageSended.MessageId, UserId = uc.TelegramId });

                if (!upd_main_msg_res.Success())
                    logger.LogError($"Попытка установить TelegramMessageId #{messageSended.MessageId} для [основного сообщения] {uc} вернула ошибку:\n{upd_main_msg_res.Message()}");
                else
                    logger.LogDebug($"TelegramMessageId для [основного сообщения] {uc} изменён на #{messageSended.MessageId}.");
#if DEBUG
                await botClient.EditMessageTextAsync(
                    chatId: messageSended.Chat.Id,
                    messageId: messageSended.MessageId,
                    text: $"#<b>{messageSended.MessageId}</b>\n{msg}",
                    parseMode: ParseMode.Html,
                    replyMarkup: (InlineKeyboardMarkup?)replyKB,
                    cancellationToken: cancellationToken);
#endif
            }
            return;
        }


        if (!resp.MainTelegramMessageId.HasValue || resp.MainTelegramMessageId == 0)
        {
            if (uc.MainTelegramMessageId.HasValue && uc.MainTelegramMessageId != default && uc.MainTelegramMessageId != 0)
            {
                try
                {
                    await botClient.DeleteMessageAsync(
                                                    chatId: chatId,
                                                    messageId: uc.MainTelegramMessageId.Value,
                                                    cancellationToken: cancellationToken);
                    logger.LogDebug($"[Основное сообщение #{uc.MainTelegramMessageId.Value}] Telegram бота для {uc} удалено (сервис обработки входящих сообщений запросил удаление).");
                    upd_main_msg_res = await webRemoteRepo
                .UpdateTelegramMainUserMessage(new() { MessageId = 0, UserId = uc.TelegramId });

                    if (!upd_main_msg_res.Success())
                        logger.LogError($"Попытка обнулить TelegramMessageId для [основного сообщения] {uc} вернула ошибку:\n{upd_main_msg_res.Message()}");
                    else
                        logger.LogDebug($"TelegramMessageId для [основного сообщения] {uc} обнулён (сервис обработки входящих сообщений запросил удаление).");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, $"Команда удаления [основного сообщения] TelegramBot (сервис обработки входящих сообщений запросил удаление) не выполнена.Ошибка удаления [основного сообщения #{uc.MainTelegramMessageId.Value}] для {uc} (вероятно оно уже было удалено ранее). warning 4B369044-6501-4BAE-9F4D-D69439D971D1");
                }
            }
            //replyKB ??= new ReplyKeyboardRemove();
            messageSended = await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: resp.Response,
                                            parseMode: ParseMode.Html,
                                            replyMarkup: replyKB,
                                            cancellationToken: cancellationToken);

#if DEBUG
            messageSended = await botClient.EditMessageTextAsync(
                chatId: messageSended.Chat.Id,
                messageId: messageSended.MessageId,
                text: $"#<b>{messageSended.MessageId}</b>\n{resp.Response}",
                parseMode: ParseMode.Html,
                replyMarkup: (InlineKeyboardMarkup?)replyKB,
                cancellationToken: cancellationToken
                );
#endif

            upd_main_msg_res = await webRemoteRepo
                .UpdateTelegramMainUserMessage(new() { MessageId = messageSended.MessageId, UserId = uc.TelegramId });

            if (!upd_main_msg_res.Success())
                logger.LogError(upd_main_msg_res.Message());
            else
                logger.LogDebug($"TelegramMessageId для [основного сообщения] сменился: {uc.MainTelegramMessageId} -> {messageSended.MessageId}");
        }
        else
        {
            try
            {
                messageSended = await botClient.EditMessageTextAsync(
                                            chatId: chatId,
                                            messageId: resp.MainTelegramMessageId.Value,
                                            text: resp.Response,
                                            parseMode: ParseMode.Html,
                                            replyMarkup: (InlineKeyboardMarkup?)replyKB,
                                            cancellationToken: cancellationToken);
                logger.LogDebug($"[Основное сообщение #{resp.MainTelegramMessageId.Value}] Telegram бота для {uc} изменено.");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Не удалось изменить [основного сообщения] для {uc} (вероятно ранее оно уже было удалено). warning 6C43B500-A863-4C3E-B4A6-238483F27166");
                replyKB ??= new ReplyKeyboardRemove();
                messageSended = await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: resp.Response,
                                            parseMode: ParseMode.Html,
                                            replyMarkup: replyKB,
                                            cancellationToken: cancellationToken);

#if DEBUG
                await botClient.EditMessageTextAsync(
                    chatId: messageSended.Chat.Id,
                    messageId: messageSended.MessageId,
                    text: $"#<b>{messageSended.MessageId}</b>\n{resp.Response}",
                    parseMode: ParseMode.Html,
                    replyMarkup: (InlineKeyboardMarkup?)replyKB,
                    cancellationToken: cancellationToken
                    );
#endif

                upd_main_msg_res = await webRemoteRepo
                .UpdateTelegramMainUserMessage(new() { MessageId = messageSended.MessageId, UserId = uc.TelegramId });

                if (!upd_main_msg_res.Success())
                    logger.LogError(upd_main_msg_res.Message());
                else
                    logger.LogDebug($"TelegramMessageId для [основного сообщения] сменился: {uc.MainTelegramMessageId} -> {messageSended.MessageId}");
            }
        }

    }

#pragma warning disable IDE0060 // Remove unused parameter
    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        string ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}