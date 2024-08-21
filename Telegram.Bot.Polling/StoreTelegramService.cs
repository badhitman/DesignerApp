////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using SharedLib;
using Telegram.Bot.Types;

namespace Telegram.Bot.Services;

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
                ChatTelegramId = chat.Id,
                FirstName = chat.FirstName,
                IsForum = chat.IsForum,
                LastName = chat.LastName,
                Title = chat.Title,
                Username = chat.Username,
                Type = (ChatsTypesTelegramEnum)(int)chat.Type,
            };

            await context.AddAsync(chat_db);
        }
        else
        {
            chat_db.ChatTelegramId = chat.Id;
            chat_db.FirstName = chat.FirstName;
            chat_db.IsForum = chat.IsForum;
            chat_db.LastName = chat.LastName;
            chat_db.Title = chat.Title;
            chat_db.Username = chat.Username;
            chat_db.Type = (ChatsTypesTelegramEnum)(int)chat.Type;
            chat_db.LastMessageUtc = DateTime.UtcNow;
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
                AddedToAttachmentMenu = user.AddedToAttachmentMenu,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsBot = user.IsBot,
                IsPremium = user.IsPremium,
                LanguageCode = user.LanguageCode,
                Username = user.Username,
                UserTelegramId = user.Id,
            };
            await context.AddAsync(user_db);
        }
        else
        {
            user_db.AddedToAttachmentMenu = user.AddedToAttachmentMenu;
            user_db.FirstName = user.FirstName;
            user_db.LastName = user.LastName;
            user_db.IsBot = user.IsBot;
            user_db.IsPremium = user.IsPremium;
            user_db.LanguageCode = user.LanguageCode;
            user_db.Username = user.Username;
            user_db.UserTelegramId = user.Id;
            user_db.LastMessageUtc = DateTime.UtcNow;

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
        MessageTelegramModelDB? messageDb = from_db is null
            ? await context.Messages.FirstOrDefaultAsync(x => x.MessageTelegramId == message.MessageId && x.ChatId == chat_db.ChatTelegramId && x.FromId == null)
            : await context.Messages.FirstOrDefaultAsync(x => x.MessageTelegramId == message.MessageId && x.ChatId == chat_db.ChatTelegramId && x.FromId == from_db.Id);

        if (messageDb is null)
        {
            messageDb = new()
            {
                ChatId = chat_db.Id,
                FromId = from_db?.Id,
                EditDate = message.EditDate,
                ForwardDate = message.ForwardDate,

                ForwardFromChatId = message.ForwardFromMessageId,
                ForwardFromMessageId = message.ForwardFromMessageId,
                ForwardFromId = forward_from_db?.Id,
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
                AuthorSignature = message.AuthorSignature,
                MediaGroupId = message.MediaGroupId,
                Text = message.Text,
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
                    MessageId = messageDb.Id,
                })];
            }

            await context.AddAsync(messageDb);
            await context.SaveChangesAsync();

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
                    MessageId = messageDb.Id,
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

                await context.AddAsync(au);
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
                    MessageId = messageDb.Id,
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

                await context.AddAsync(dt);
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
                    MessageId = messageDb.Id,
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

                await context.AddAsync(vt);
            }

            if (message.Voice is not null)
                await context.AddAsync(new VoiceTelegramModelDB()
                {
                    FileId = message.Voice.FileId,
                    FileUniqueId = message.Voice.FileId,
                    FileSize = message.Voice.FileSize,
                    MimeType = message.Voice.MimeType,
                    Duration = message.Voice.Duration,
                    MessageId = messageDb.Id,
                });

            if (message.Contact is not null)
                await context.AddAsync(new ContactTelegramModelDB()
                {
                    FirstName = message.Contact.FirstName,
                    LastName = message.Contact.LastName,
                    PhoneNumber = message.Contact.PhoneNumber,
                    Vcard = message.Contact.Vcard,
                    UserId = message.Contact.UserId,
                    MessageId = messageDb.Id,
                });

            if (message.Audio is not null || message.Document is not null || message.Video is not null || message.Voice is not null || message.Contact is not null)
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
            messageDb.AuthorSignature = message.AuthorSignature;
            messageDb.MediaGroupId = message.MediaGroupId;
            messageDb.Text = message.Text;

            context.Update(messageDb);
            await context.SaveChangesAsync();
        }

        await context
            .Chats
            .Where(x => x.Id == chat_db.Id)
            .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastMessageId, messageDb.Id));

        if (sender_chat_db is not null && sender_chat_db.Id != chat_db.Id)
            await context
                .Chats.Where(x => x.Id == sender_chat_db.Id)
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastMessageId, messageDb.Id));

        if (from_db is not null)
            await context
                .Users.Where(x => x.Id == from_db.Id)
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastMessageId, messageDb.Id));

        return messageDb;
    }
}