////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Получить чаты
/// </summary>
public class ChatsSelectTelegramReceive(ITelegramBotService tgRepo) 
    : IResponseReceive<TPaginationRequestModel<string?>?, TPaginationResponseModel<ChatTelegramModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ChatsSelectTelegramReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<ChatTelegramModelDB>?> ResponseHandleAction(TPaginationRequestModel<string?>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await tgRepo.ChatsSelectTelegram(req);
    }
}