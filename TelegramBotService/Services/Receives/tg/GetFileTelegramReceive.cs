////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Получить файл из Telegram
/// </summary>
public class GetFileTelegramReceive(ITelegramBotService tgRepo)
    : IResponseReceive<string?, TResponseModel<byte[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadFileTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<byte[]>?> ResponseHandleAction(string? fileId)
    {
        ArgumentNullException.ThrowIfNull(fileId);
        return await tgRepo.GetFileTelegram(fileId);
    }
}