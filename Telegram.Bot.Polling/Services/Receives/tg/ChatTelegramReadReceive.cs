////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Прочитать данные чата
/// </summary>
public class ChatTelegramReadReceive(IDbContextFactory<TelegramBotContext> tgDbFactory)
    : IResponseReceive<int, ChatTelegramModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ChatReadTelegramReceive;

    /// <inheritdoc/>
    public async Task<ChatTelegramModelDB?> ResponseHandleAction(int chat_id)
    {
        TResponseModel<ChatTelegramModelDB> res = new();
        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
        return await context
            .Chats
            .Include(x => x.UsersJoins!)
            .ThenInclude(x => x.User)
            .FirstAsync(x => x.Id == chat_id);
    }
}