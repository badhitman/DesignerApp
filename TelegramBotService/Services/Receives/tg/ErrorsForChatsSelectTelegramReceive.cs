////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Получить ошибки отправок сообщений (для чатов)
/// </summary>
public class ErrorsForChatsSelectTelegramReceive(ITelegramBotService tgRepo)
    : IResponseReceive<TPaginationRequestModel<long[]>?, TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ErrorsForChatsSelectTelegramReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>?> ResponseHandleAction(TPaginationRequestModel<long[]>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await tgRepo.ErrorsForChatsSelectTelegram(req);
    }
}