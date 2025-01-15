////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Отправить сообщение пользователю через TelegramBot SendTextMessageTelegramBotModel
/// </summary>
public class SendTextMessageTelegramReceive(ITelegramBotService tgRepo, ILogger<SendTextMessageTelegramReceive> _logger)
    : IResponseReceive<SendTextMessageTelegramBotModel?, TResponseModel<MessageComplexIdsModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SendTextMessageTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<MessageComplexIdsModel>?> ResponseHandleAction(SendTextMessageTelegramBotModel? message)
    {
        ArgumentNullException.ThrowIfNull(message);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(message)}");
        return await tgRepo.SendTextMessageTelegram(message);
    }
}