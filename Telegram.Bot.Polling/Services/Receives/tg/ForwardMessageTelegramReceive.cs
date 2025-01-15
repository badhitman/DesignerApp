////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Переслать сообщение пользователю через TelegramBot ForwardMessageTelegramReceive
/// </summary>
public class ForwardMessageTelegramReceive(ILogger<ForwardMessageTelegramReceive> loggerRepo, ITelegramBotService tgRepo)
    : IResponseReceive<ForwardMessageTelegramBotModel?, TResponseModel<MessageComplexIdsModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ForwardTextMessageTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<MessageComplexIdsModel>?> ResponseHandleAction(ForwardMessageTelegramBotModel? message)
    {
        ArgumentNullException.ThrowIfNull(message);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(message)}");
        return await tgRepo.ForwardMessageTelegram(message);
    }
}