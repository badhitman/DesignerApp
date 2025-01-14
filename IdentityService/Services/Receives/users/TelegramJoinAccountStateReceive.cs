////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Получить состояние процедуры привязки аккаунта Telegram к учётной записи сайта (если есть).
/// Если userId не указан, то команда выполняется для текущего пользователя (запрос/сессия)
/// </summary>
public class TelegramJoinAccountStateReceive(IIdentityTools idRepo, ILogger<TelegramJoinAccountStateReceive> loggerRepo)
    : IResponseReceive<TelegramJoinAccountStateRequestModel?, TResponseModel<TelegramJoinAccountModelDb>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountStateReceive;

    /// <summary>
    /// Получить состояние процедуры привязки аккаунта Telegram к учётной записи сайта (если есть).
    /// Если userId не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public async Task<TResponseModel<TelegramJoinAccountModelDb>?> ResponseHandleAction(TelegramJoinAccountStateRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.TelegramJoinAccountState(req);
    }
}