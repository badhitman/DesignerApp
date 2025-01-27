////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Прочитать чаты
/// </summary>
public class ChatsReadTelegramReceive(ITelegramBotService tgRepo)
    : IResponseReceive<long[]?, List<ChatTelegramModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ChatsReadTelegramReceive;

    /// <inheritdoc/>
    public async Task<List<ChatTelegramModelDB>?> ResponseHandleAction(long[]? chats_ids)
    {
        ArgumentNullException.ThrowIfNull(chats_ids);
        return await tgRepo.ChatsReadTelegram(chats_ids);
    }
}