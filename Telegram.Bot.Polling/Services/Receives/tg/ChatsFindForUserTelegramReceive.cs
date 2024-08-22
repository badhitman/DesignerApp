////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Найти чаты пользователей
/// </summary>
public class ChatsFindForUserTelegramReceive(IDbContextFactory<TelegramBotContext> tgDbFactory)
    : IResponseReceive<long[]?, ChatTelegramModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ChatsFindForUserTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<ChatTelegramModelDB[]?>> ResponseHandleAction(long[]? chats_ids)
    {
        ArgumentNullException.ThrowIfNull(chats_ids);
        TResponseModel<ChatTelegramModelDB[]?> res = new();
        TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();

        int[] users_ids = await context.Users.Where(x => chats_ids.Contains(x.UserTelegramId)).Select(x => x.Id).ToArrayAsync();

        IQueryable<ChatTelegramModelDB> q = users_ids.Length == 0
            ? context.Chats.Where(x => chats_ids.Contains(x.ChatTelegramId))
            : context.Chats.Where(x => chats_ids.Contains(x.ChatTelegramId) || context.JoinsUsersToChats.Any(y => y.ChatId == x.Id && users_ids.Contains(y.UserId)));

        res.Response = await q
            .Include(x => x.UsersJoins!)
            .ThenInclude(x => x.User)
            .ToArrayAsync();

        return res;
    }
}