////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Прочитать чаты
/// </summary>
public class ChatsReadTelegramReceive(IDbContextFactory<TelegramBotContext> tgDbFactory)
    : IResponseReceive<long[]?, ChatTelegramModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ChatsReadTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<ChatTelegramModelDB[]?>> ResponseHandleAction(long[]? chats_ids)
    {
        ArgumentNullException.ThrowIfNull(chats_ids);
        TResponseModel<ChatTelegramModelDB[]?> res = new();
        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
        res.Response = await context
            .Chats
            .Where(x => chats_ids.Contains(x.ChatTelegramId))
            .ToArrayAsync();

        return res;
    }
}