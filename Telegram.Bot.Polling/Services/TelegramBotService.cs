////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.Query;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using SharedLib;
using DbcLib;

namespace Telegram.Bot.Services;

/// <summary>
/// TelegramBotService
/// </summary>
public class TelegramBotService(ILogger<TelegramBotService> _logger,
    ITelegramBotClient _botClient,
    IDbContextFactory<TelegramBotContext> tgDbFactory,
    IIdentityTransmission IdentityRepo,
    StoreTelegramService storeTgRepo) : ITelegramBotService
{
    /// <inheritdoc/>
    public async Task<List<ChatTelegramModelDB>> ChatsFindForUserTelegram(long[] chats_ids)
    {
        TResponseModel<ChatTelegramModelDB[]?> res = new();
        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();

        int[] users_ids = await context.Users.Where(x => chats_ids.Contains(x.UserTelegramId)).Select(x => x.Id).ToArrayAsync();

        IQueryable<ChatTelegramModelDB> q = users_ids.Length == 0
            ? context.Chats.Where(x => chats_ids.Contains(x.ChatTelegramId))
            : context.Chats.Where(x => chats_ids.Contains(x.ChatTelegramId) || context.JoinsUsersToChats.Any(y => y.ChatId == x.Id && users_ids.Contains(y.UserId)));

        return await q
            .Include(x => x.UsersJoins!)
            .ThenInclude(x => x.User)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<ChatTelegramModelDB>> ChatsReadTelegram(long[] chats_ids)
    {
        TResponseModel<ChatTelegramModelDB[]> res = new();
        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
        return await context.Chats.Where(x => chats_ids.Contains(x.ChatTelegramId)).ToListAsync()
        ;
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<ChatTelegramModelDB>> ChatsSelectTelegram(TPaginationRequestModel<string?> req)
    {
        if (req.PageSize < 5)
            req.PageSize = 5;

        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();

        IQueryable<ChatTelegramModelDB> q = context
            .Chats
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload))
        {
            string find_req = req.Payload.ToUpper();
            q = q.Where(x =>
            (x.NormalizedFirstNameUpper != null && x.NormalizedFirstNameUpper.Contains(find_req)) ||
            (x.NormalizedLastNameUpper != null && x.NormalizedLastNameUpper.Contains(find_req)) ||
            (x.NormalizedTitleUpper != null && x.NormalizedTitleUpper.Contains(find_req)) ||
            (x.NormalizedUsernameUpper != null && x.NormalizedUsernameUpper.Contains(find_req))
            );
        }

        IQueryable<ChatTelegramModelDB> TakePart(IQueryable<ChatTelegramModelDB> q, VerticalDirectionsEnum direct)
        {
            return direct == VerticalDirectionsEnum.Up
                ? q.OrderBy(x => x.LastUpdateUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize)
                : q.OrderByDescending(x => x.LastUpdateUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize);
        }

        return new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection,
            TotalRowsCount = await q.CountAsync(),
            Response = await TakePart(q, req.SortingDirection).ToListAsync(),
        };
    }

    /// <inheritdoc/>
    public async Task<ChatTelegramModelDB> ChatTelegramRead(int chatId)
    {
        TResponseModel<ChatTelegramModelDB> res = new();
        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
        return await context
            .Chats
            .Include(x => x.UsersJoins!)
            .ThenInclude(x => x.User)
            .FirstAsync(x => x.Id == chatId);
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>> ErrorsForChatsSelectTelegram(TPaginationRequestModel<long[]> req)
    {
        if (req.PageSize < 5)
            req.PageSize = 5;

        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();

        IQueryable<ErrorSendingMessageTelegramBotModelDB> q = context
            .ErrorsSendingTextMessageTelegramBot
            .AsQueryable();

        if (req.Payload is not null && req.Payload.Length != 0)
            q = q.Where(x => req.Payload.Any(y => y == x.ChatId));

        IQueryable<ErrorSendingMessageTelegramBotModelDB> TakePart(IQueryable<ErrorSendingMessageTelegramBotModelDB> q, VerticalDirectionsEnum direct)
        {
            return direct == VerticalDirectionsEnum.Up
                ? q.OrderBy(x => x.CreatedAtUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize)
                : q.OrderByDescending(x => x.CreatedAtUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize);
        }

        return new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection,
            TotalRowsCount = await q.CountAsync(),
            Response = await TakePart(q, req.SortingDirection).ToListAsync(),
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<MessageComplexIdsModel>> ForwardMessageTelegram(ForwardMessageTelegramBotModel message)
    {
        TResponseModel<MessageComplexIdsModel> res = new();
        Message sender_msg;
        try
        {
            sender_msg = await _botClient.ForwardMessage(chatId: message.DestinationChatId, fromChatId: message.SourceChatId, messageId: message.SourceMessageId);

            MessageTelegramModelDB msg_db = await storeTgRepo.StoreMessage(sender_msg);
            res.Response = new()
            {
                TelegramId = sender_msg.MessageId,
                DatabaseId = msg_db.Id,
            };
        }
        catch (Exception ex)
        {
            int? errorCode = null;
            if (ex is ApiRequestException _are)
                errorCode = _are.ErrorCode;
            else if (ex is RequestException _re)
                errorCode = (int?)_re.HttpStatusCode;

            using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
            await context.AddAsync(new ErrorSendingMessageTelegramBotModelDB()
            {
                ChatId = message.DestinationChatId,
                CreatedAtUtc = DateTime.UtcNow,
                SourceMessageId = message.SourceMessageId,
                Message = ex.Message,
                ExceptionTypeName = ex.GetType().FullName,
                ErrorCode = errorCode,
            });
            await context.SaveChangesAsync();

            res.AddError("Ошибка отправки Telegram сообщения. error E06E939D-6E93-45CE-A5F5-19A417A27DC1");

            res.Messages.InjectException(ex);
            return res;
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string>> GetBotUsername()
    {
        TResponseModel<string> res = new();
        Telegram.Bot.Types.User me;
        string msg;
        try
        {
            me = await _botClient.GetMe();
        }
        catch (Exception ex)
        {
            msg = "Ошибка получения данных бота `_botClient.GetMe`. error {50EE48C7-5A8A-420B-8B71-D1E2E44E48F4}";
            _logger.LogError(ex, msg);
            res.Messages.InjectException(ex);
            return res;
        }
        res.Response = me.Username;
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<byte[]>> GetFileTelegram(string fileId)
    {
        TResponseModel<byte[]> res = new();
        try
        {
            Types.File fileTg = await _botClient.GetFile(fileId);
            MemoryStream ms = new();

            if (string.IsNullOrWhiteSpace(fileTg.FilePath))
            {
                res.AddError($"Ошибка получения {nameof(fileTg.FilePath)}");
                return res;
            }

            await _botClient.DownloadFile(fileTg.FilePath, ms);
            res.Response = ms.ToArray();
        }
        catch (Exception ex)
        {
            res.AddError(ex.Message);
        }
        return res;
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<MessageTelegramModelDB>> MessagesSelectTelegram(TPaginationRequestModel<SearchMessagesChatModel> req)
    {
        if (req.PageSize < 5)
            req.PageSize = 5;

        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
        IQueryable<MessageTelegramModelDB> q = context.Messages.AsQueryable();

        if (req.Payload.ChatId != 0)
            q = q.Where(x => x.ChatId == req.Payload.ChatId);

        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
        {
            req.Payload.SearchQuery = req.Payload.SearchQuery.ToUpper();
            q = q.Where(x => (x.NormalizedTextUpper != null && x.NormalizedTextUpper.Contains(req.Payload.SearchQuery)) || (x.NormalizedCaptionUpper != null && x.NormalizedCaptionUpper.Contains(req.Payload.SearchQuery)));
        }

        async Task<List<MessageTelegramModelDB>> Include(IQueryable<MessageTelegramModelDB> query)
        {
            var dbData = await
                (from msg in query

                 join joinDoc in context.Documents on msg.DocumentId equals joinDoc.MessageId into getDoc
                 from document in getDoc.DefaultIfEmpty()

                 join joinAudio in context.Audios on msg.DocumentId equals joinAudio.MessageId into getAudio
                 from audio in getAudio.DefaultIfEmpty()

                 join joinVoice in context.Voices on msg.DocumentId equals joinVoice.MessageId into getVoice
                 from voice in getVoice.DefaultIfEmpty()

                 join joinVideo in context.Videos on msg.DocumentId equals joinVideo.MessageId into getVideo
                 from video in getVideo.DefaultIfEmpty()

                 join joinSender in context.Users on msg.FromId equals joinSender.Id into getSender
                 from sender in getSender.DefaultIfEmpty()

                 join joinForward in context.Users on msg.ForwardFromId equals joinForward.UserTelegramId into getForward
                 from forward in getForward.DefaultIfEmpty()

                 join joinChat in context.Chats on msg.ChatId equals joinChat.Id into getChat
                 from chat in getChat.DefaultIfEmpty()

                 select new { msg, document, audio, voice, video, sender, forward, chat }).ToListAsync();

            dbData.ForEach(r =>
            {
                r.msg.ForwardFrom = r.forward;
                r.msg.Chat = r.chat;
                r.msg.From = r.sender;
                r.msg.Document = r.document;
                r.msg.Voice = r.voice;  
                r.msg.Video = r.video;  
                r.msg.Audio = r.audio;
            });

            return dbData.Select(x => x.msg).ToList();
        }

        IQueryable<MessageTelegramModelDB> TakePart(IQueryable<MessageTelegramModelDB> q, VerticalDirectionsEnum direct)
        {
            return direct == VerticalDirectionsEnum.Up
                ? q.OrderBy(x => x.CreatedAtUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize)
                : q.OrderByDescending(x => x.CreatedAtUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize);
        }

        return new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection,
            TotalRowsCount = await q.CountAsync(),
            Response = await Include(TakePart(q, req.SortingDirection))
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<MessageComplexIdsModel>> SendTextMessageTelegram(SendTextMessageTelegramBotModel message)
    {
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
        MessageTelegramModelDB msg_db;
        try
        {
            string msg_text = string.IsNullOrWhiteSpace(message.From)
            ? message.Message
                : $"{message.Message}\n--- {message.From.Trim()}";

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

            //sender_msg = new()
            //{
            //  Type =  MessageType.,
            //};
            //msg_db = await storeTgRepo.StoreMessage(sender_msg);

            await context.AddAsync(new ErrorSendingMessageTelegramBotModelDB()
            {
                ChatId = message.UserTelegramId,
                CreatedAtUtc = DateTime.UtcNow,
                ReplyToMessageId = message.ReplyToMessageId,
                ParseModeName = message.ParseModeName,
                SignFrom = message.From,
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
            await IdentityRepo.UpdateTelegramMainUserMessage(new() { MessageId = 0, UserId = message.UserTelegramId });
        }

        return res;
    }
}