////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Найти чаты пользователей
/// </summary>
public class ChatsFindForUserTelegramReceive(ITelegramBotService tgRepo)
    : IResponseReceive<long[]?, List<ChatTelegramModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ChatsFindForUserTelegramReceive;

    /// <inheritdoc/>
    public async Task<List<ChatTelegramModelDB>?> ResponseHandleAction(long[]? chats_ids)
    {
        ArgumentNullException.ThrowIfNull(chats_ids);
        return await tgRepo.ChatsFindForUserTelegram(chats_ids);
    }
}