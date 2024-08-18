////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using SharedLib;
using Telegram.Bot.Types;

namespace Telegram.Bot.Services;

/// <summary>
/// StoreTelegramService
/// </summary>
public class StoreTelegramService(IDbContextFactory<TelegramBotContext> helpdeskDbFactory)
{
    /// <summary>
    /// Store Chat
    /// </summary>
    public async Task<ChatTelegramModelDB> StoreChat(Chat chat)
    {
        using TelegramBotContext context = await helpdeskDbFactory.CreateDbContextAsync();
        ChatTelegramModelDB? chat_db = await context
            .Chats
            .FirstOrDefaultAsync(x => x.ChatId == chat.Id);

        if (chat_db is null)
        {
            chat_db = new ChatTelegramModelDB()
            {
                ChatId = chat.Id,
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
            chat_db.ChatId = chat.Id;
            chat_db.FirstName = chat.FirstName;
            chat_db.IsForum = chat.IsForum;
            chat_db.LastName = chat.LastName;
            chat_db.Title = chat.Title;
            chat_db.Username = chat.Username;
            chat_db.Type = (ChatsTypesTelegramEnum)(int)chat.Type;
            context.Update(chat_db);
        }
        await context.SaveChangesAsync();
        return chat_db;
    }

    /// <summary>
    /// StoreUser
    /// </summary>
    public async Task<UserTelegramModelDB> StoreUser(User user)
    {
        using TelegramBotContext context = await helpdeskDbFactory.CreateDbContextAsync();
        UserTelegramModelDB? user_db = await context
            .Users
            .FirstOrDefaultAsync(x => x.TelegramId == user.Id);

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
                TelegramId = user.Id,
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
            user_db.TelegramId = user.Id;

            context.Update(user_db);
        }
        await context.SaveChangesAsync();
        return user_db;
    }
}

//MessageTelegramModelDB? messageDb = await context
//    .Messages
//    .FirstOrDefaultAsync(x => x.MessageId == message.MessageId && x.ChatId == chat_db.ChatId && x.FromId == user_db.TelegramId, cancellationToken: cancellationToken);

//if (messageDb is null)
//{
//    messageDb = new()
//    {
//        ChatId = chat_db.Id,
//        FromId = user_db.Id,
//        EditDate = message.EditDate,
//        ForwardDate = message.ForwardDate,

//        ForwardFromChatId = message.ForwardFromMessageId,
//        ForwardFromMessageId = message.ForwardFromMessageId,
//        ForwardFromId = message.ForwardFrom?.Id,
//        ForwardSenderName = message.ForwardSenderName,
//        ForwardSignature = message.ForwardSignature,

//        IsAutomaticForward = message.IsAutomaticForward,
//        MessageId = message.MessageId,
//        MessageThreadId = message.MessageThreadId,

//        ViaBotUserId = message.ViaBot?.Id,
//        IsTopicMessage = message.IsTopicMessage,
//        SenderChatId = message.SenderChat?.Id,
//    };
//    await context.AddAsync(messageDb, cancellationToken);
//}
//else
//{

//}

//await context.SaveChangesAsync(cancellationToken);