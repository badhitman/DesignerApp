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
    : IResponseReceive<int?, ChatTelegramModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ChatReadTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<ChatTelegramModelDB?>> ResponseHandleAction(int? chat_id)
    {
        ArgumentNullException.ThrowIfNull(chat_id);
        TResponseModel<ChatTelegramModelDB?> res = new();
        TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();

        res.Response = await context
            .Chats
            .Include(x => x.UsersJoins!)
            .ThenInclude(x => x.User)
            .FirstAsync(x => x.Id == chat_id);

        return res;
    }
}