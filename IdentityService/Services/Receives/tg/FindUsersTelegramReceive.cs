////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Telegram пользователи (сохранённые).
/// Все пользователи, которые когда либо писали что либо в бота - сохраняются/кэшируются в БД.
/// </summary>
public class FindUsersTelegramReceive(IIdentityTools identityRepo)
    : IResponseReceive<FindRequestModel?, TPaginationResponseModel<TelegramUserViewModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FindUsersTelegramReceive;

    /// <summary>
    /// Telegram пользователи (сохранённые).
    /// Все пользователи, которые когда либо писали что либо в бота - сохраняются/кэшируются в БД.
    /// </summary>
    public async Task<TPaginationResponseModel<TelegramUserViewModel>?> ResponseHandleAction(FindRequestModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await identityRepo.FindUsersTelegram(payload);
    }
}