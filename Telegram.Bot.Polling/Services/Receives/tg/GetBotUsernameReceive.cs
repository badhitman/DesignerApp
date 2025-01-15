////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Получить Username TelegramBot
/// </summary>
public class GetBotUsernameReceive(ITelegramBotService tgRepo)
    : IResponseReceive<object?, TResponseModel<string>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetBotUsernameReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<string>?> ResponseHandleAction(object? payload = null)
    {
        return await tgRepo.GetBotUsername();
    }
}