////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using SharedLib;
using DbcLib;

namespace TelegramBotService;

/// <summary>
/// Сохранение в базу данных данных Telegram
/// </summary>
public class StoreTelegramService(IDbContextFactory<TelegramBotContext> tgDbFactory)
{
    /// <summary>
    /// Сохранить чат в базу данных
    /// </summary>
    public async Task<ChatTelegramModelDB> StoreChat(Chat chat)
    {
        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
        ChatTelegramModelDB? chat_db = await context
            .Chats
            .Include(x => x.ChatPhoto)
            .FirstOrDefaultAsync(x => x.ChatTelegramId == chat.Id);

        if (chat_db is null)
        {
            chat_db = new ChatTelegramModelDB()
            {
                Type = (ChatsTypesTelegramEnum)(int)chat.Type,
                ChatTelegramId = chat.Id,
                IsForum = chat.IsForum,

                Title = chat.Title,
                NormalizedTitleUpper = chat.Title?.ToUpper(),

                FirstName = chat.FirstName,
                NormalizedFirstNameUpper = chat.FirstName?.ToUpper(),

                LastName = chat.LastName,
                NormalizedLastNameUpper = chat.LastName?.ToUpper(),

                Username = chat.Username,
                NormalizedUsernameUpper = chat.Username?.ToUpper(),
            };

            await context.AddAsync(chat_db);
        }
        else
        {
            chat_db.Type = (ChatsTypesTelegramEnum)(int)chat.Type;
            chat_db.ChatTelegramId = chat.Id;
            chat_db.IsForum = chat.IsForum;

            chat_db.Title = chat.Title;
            chat_db.NormalizedTitleUpper = chat.Title?.ToUpper();

            chat_db.FirstName = chat.FirstName;
            chat_db.NormalizedFirstNameUpper = chat.FirstName?.ToUpper();

            chat_db.LastName = chat.LastName;
            chat_db.NormalizedLastNameUpper = chat.LastName?.ToUpper();

            chat_db.Username = chat.Username;
            chat_db.NormalizedUsernameUpper = chat.Username?.ToUpper();

            chat_db.LastUpdateUtc = DateTime.UtcNow;
            context.Update(chat_db);
        }
        await context.SaveChangesAsync();
        return chat_db;
    }

    /// <summary>
    /// Сохранить пользователя в базу данных
    /// </summary>
    public async Task<UserTelegramModelDB> StoreUser(User user)
    {
        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
        UserTelegramModelDB? user_db = await context
            .Users
            .FirstOrDefaultAsync(x => x.UserTelegramId == user.Id);

        if (user_db is null)
        {
            user_db = new()
            {
                FirstName = user.FirstName,
                NormalizedFirstNameUpper = user.FirstName.ToUpper(),

                LastName = user.LastName,
                NormalizedLastNameUpper = user.LastName?.ToUpper(),

                Username = user.Username,
                NormalizedUsernameUpper = user.Username?.ToUpper(),

                UserTelegramId = user.Id,
                IsBot = user.IsBot,
                IsPremium = user.IsPremium,

                AddedToAttachmentMenu = user.AddedToAttachmentMenu,
                LanguageCode = user.LanguageCode,
            };
            await context.AddAsync(user_db);
        }
        else
        {
            user_db.FirstName = user.FirstName;
            user_db.NormalizedFirstNameUpper = user.FirstName.ToUpper();

            user_db.LastName = user.LastName;
            user_db.NormalizedLastNameUpper = user.LastName?.ToUpper();

            user_db.Username = user.Username;
            user_db.NormalizedUsernameUpper = user.Username?.ToUpper();

            user_db.UserTelegramId = user.Id;
            user_db.IsBot = user.IsBot;
            user_db.IsPremium = user.IsPremium;

            user_db.AddedToAttachmentMenu = user.AddedToAttachmentMenu;
            user_db.LanguageCode = user.LanguageCode;
            user_db.LastUpdateUtc = DateTime.UtcNow;

            context.Update(user_db);
        }
        await context.SaveChangesAsync();
        return user_db;
    }

    /// <summary>
    /// Сохранить сообщение в базу данных
    /// </summary>
    public async Task<MessageTelegramModelDB> StoreMessage(Message message)
    {
        ChatTelegramModelDB chat_db = await StoreChat(message.Chat);
        ChatTelegramModelDB? sender_chat_db = message.SenderChat is null ? null : await StoreChat(message.SenderChat);
        UserTelegramModelDB? from_db = message.From is null ? null : await StoreUser(message.From);
        UserTelegramModelDB? forward_from_db = message.ForwardFrom is null ? null : await StoreUser(message.ForwardFrom);

        MessageTelegramModelDB? replyToMessageDB = message.ReplyToMessage is null ? null : await StoreMessage(message.ReplyToMessage);

        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();

        IIncludableQueryable<MessageTelegramModelDB, List<PhotoMessageTelegramModelDB>?> q = context
            .Messages
            .Include(x => x.Audio)
            .Include(x => x.Chat)
            .Include(x => x.Voice)
            .Include(x => x.Contact)
            .Include(x => x.Document)
            .Include(x => x.From)
            .Include(x => x.Photo);

        MessageTelegramModelDB? messageDb = from_db is null
            ? await q.FirstOrDefaultAsync(x => x.MessageTelegramId == message.MessageId && x.ChatId == chat_db.ChatTelegramId && x.FromId == null)
            : await q.FirstOrDefaultAsync(x => x.MessageTelegramId == message.MessageId && x.ChatId == chat_db.ChatTelegramId && x.FromId == from_db.Id);

        if (messageDb is null)
        {
            messageDb = new()
            {
                ChatId = chat_db.Id,
                FromId = from_db?.Id,
                EditDate = message.EditDate,
                ForwardDate = message.ForwardDate,

                ForwardFromChatId = message.ForwardFromChat?.Id,
                ForwardFromMessageId = message.ForwardFromMessageId,
                ForwardFromId = message.ForwardFrom?.Id,
                ForwardSenderName = message.ForwardSenderName,
                ForwardSignature = message.ForwardSignature,

                IsAutomaticForward = message.IsAutomaticForward,
                MessageTelegramId = message.MessageId,
                MessageThreadId = message.MessageThreadId,

                ViaBotId = message.ViaBot?.Id,
                IsTopicMessage = message.IsTopicMessage,
                SenderChatId = sender_chat_db?.Id,
                ReplyToMessageId = replyToMessageDB?.Id,
                //
                Caption = message.Caption,
                NormalizedCaptionUpper = message.Caption?.ToUpper(),

                Text = message.Text,
                NormalizedTextUpper = message.Text?.ToUpper(),

                AuthorSignature = message.AuthorSignature,
                MediaGroupId = message.MediaGroupId,
            };

            if (message.Photo is not null && message.Photo.Length != 0)
            {
                messageDb.Photo = [..message.Photo.Select(x => new PhotoMessageTelegramModelDB()
                {
                    FileId = x.FileId,
                    FileUniqueId = x.FileUniqueId,
                    FileSize = x.FileSize,
                    Height = x.Height,
                    Width = x.Width,
                    Message = messageDb,
                })];
            }

            if (message.Document is not null)
            {
                DocumentTelegramModelDB dt = new()
                {
                    FileId = message.Document.FileId,
                    FileUniqueId = message.Document.FileId,
                    FileName = message.Document.FileName,
                    FileSize = message.Document.FileSize,
                    MimeType = message.Document.MimeType,
                    Message = messageDb,
                };

                if (message.Document.Thumbnail is not null)
                    dt.ThumbnailDocument = new()
                    {
                        FileId = message.Document.Thumbnail.FileId,
                        FileUniqueId = message.Document.Thumbnail.FileUniqueId,
                        MessageId = messageDb.Id,
                        DocumentOwner = dt,
                        FileSize = message.Document.Thumbnail.FileSize,
                        Width = message.Document.Thumbnail.Width,
                        Height = message.Document.Thumbnail.Height,
                    };

                messageDb.Document = dt;
            }

            if (message.Audio is not null)
            {
                AudioTelegramModelDB au = new()
                {
                    FileId = message.Audio.FileId,
                    FileUniqueId = message.Audio.FileId,
                    Duration = message.Audio.Duration,
                    FileName = message.Audio.FileName,
                    FileSize = message.Audio.FileSize,
                    MimeType = message.Audio.MimeType,
                    Title = message.Audio.Title,
                    Performer = message.Audio.Performer,
                    Message = messageDb,
                };

                if (message.Audio.Thumbnail is not null)
                    au.AudioThumbnail = new()
                    {
                        FileId = message.Audio.Thumbnail.FileId,
                        FileUniqueId = message.Audio.Thumbnail.FileId,
                        FileSize = message.Audio.Thumbnail.FileSize,
                        MessageId = messageDb.Id,
                        Width = message.Audio.Thumbnail.Width,
                        Height = message.Audio.Thumbnail.Height,
                        AudioOwner = au
                    };

                messageDb.Audio = au;
            }

            if (message.Video is not null)
            {
                VideoTelegramModelDB vt = new()
                {
                    FileId = message.Video.FileId,
                    FileUniqueId = message.Video.FileId,
                    FileName = message.Video.FileName,
                    FileSize = message.Video.FileSize,
                    MimeType = message.Video.MimeType,
                    Duration = message.Video.Duration,
                    Height = message.Video.Height,
                    Width = message.Video.Width,
                    Message = messageDb,
                };

                if (message.Video.Thumbnail is not null)
                    vt.ThumbnailVideo = new()
                    {
                        FileId = message.Video.Thumbnail.FileId,
                        FileUniqueId = message.Video.Thumbnail.FileUniqueId,
                        MessageId = messageDb.Id,
                        FileSize = message.Video.Thumbnail.FileSize,
                        Height = message.Video.Thumbnail.Height,
                        Width = message.Video.Thumbnail.Width,
                        VideoOwner = vt
                    };

                messageDb.Video = vt;
            }

            if (message.Voice is not null)
            {
                VoiceTelegramModelDB vt = new()
                {
                    FileId = message.Voice.FileId,
                    FileUniqueId = message.Voice.FileId,
                    FileSize = message.Voice.FileSize,
                    MimeType = message.Voice.MimeType,
                    Duration = message.Voice.Duration,
                    Message = messageDb,
                };
                messageDb.Voice = vt;
            }

            if (message.Contact is not null)
            {
                ContactTelegramModelDB ct = new()
                {
                    FirstName = message.Contact.FirstName,
                    LastName = message.Contact.LastName,
                    PhoneNumber = message.Contact.PhoneNumber,
                    Vcard = message.Contact.Vcard,
                    UserId = message.Contact.UserId,
                    Message = messageDb,
                };
                messageDb.Contact = ct;
            }

            await context.AddAsync(messageDb);

            await context.SaveChangesAsync();
        }
        else
        {
            messageDb.ChatId = chat_db.Id;
            messageDb.FromId = from_db?.Id;
            messageDb.EditDate = message.EditDate;
            messageDb.ForwardDate = message.ForwardDate;

            messageDb.ForwardFromChatId = message.ForwardFromMessageId;
            messageDb.ForwardFromMessageId = message.ForwardFromMessageId;
            messageDb.ForwardFromId = forward_from_db?.Id;
            messageDb.ForwardSenderName = message.ForwardSenderName;
            messageDb.ForwardSignature = message.ForwardSignature;

            messageDb.IsAutomaticForward = message.IsAutomaticForward;
            messageDb.MessageTelegramId = message.MessageId;
            messageDb.MessageThreadId = message.MessageThreadId;

            messageDb.ViaBotId = message.ViaBot?.Id;
            messageDb.IsTopicMessage = message.IsTopicMessage;
            messageDb.SenderChatId = sender_chat_db?.Id;
            messageDb.ReplyToMessageId = replyToMessageDB?.Id;
            //
            messageDb.Caption = message.Caption;
            messageDb.NormalizedCaptionUpper = message.Caption?.ToUpper();

            messageDb.Text = message.Text;
            messageDb.NormalizedTextUpper = message.Text?.ToUpper();

            messageDb.AuthorSignature = message.AuthorSignature;
            messageDb.MediaGroupId = message.MediaGroupId;

            context.Update(messageDb);
            await context.SaveChangesAsync();
        }

        await context
            .Chats
            .Where(x => x.Id == chat_db.Id)
            .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastMessageId, messageDb.Id));

        if (sender_chat_db is not null && sender_chat_db.Id != chat_db.Id)
            await context
                .Chats
                .Where(x => x.Id == sender_chat_db.Id)
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastMessageId, messageDb.Id));

        if (from_db is not null)
            await context
                .Users
                .Where(x => x.Id == from_db.Id)
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastMessageId, messageDb.Id));

        if (from_db is not null && from_db.UserTelegramId != chat_db.ChatTelegramId && !await context.JoinsUsersToChats.AnyAsync(x => x.UserId == from_db.Id && x.ChatId == chat_db.Id))
        {
            await context.AddAsync(new JoinUserChatModelDB() { ChatId = chat_db.Id, UserId = from_db.Id });
            await context.SaveChangesAsync();
        }

        messageDb.Chat = chat_db;
        messageDb.From = from_db;
        messageDb.SenderChat = sender_chat_db;
        messageDb.ForwardFrom = forward_from_db;
        messageDb.ReplyToMessage = replyToMessageDB;

        return messageDb;
    }
}