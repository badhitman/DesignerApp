////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Генерирует коды восстановления для пользователя, что делает недействительными все предыдущие коды восстановления для пользователя.
/// </summary>
/// <returns>Новые коды восстановления для пользователя. Примечание. Возвращенное число может быть меньше, поскольку дубликаты будут удалены.</returns>
public class GenerateNewTwoFactorRecoveryCodesReceive(IIdentityTools idRepo, ILogger<GenerateNewTwoFactorRecoveryCodesReceive> loggerRepo)
    : IResponseReceive<GenerateNewTwoFactorRecoveryCodesRequestModel?, TResponseModel<IEnumerable<string>?>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GenerateNewTwoFactorRecoveryCodesReceive;

    /// <summary>
    /// Генерирует коды восстановления для пользователя, что делает недействительными все предыдущие коды восстановления для пользователя.
    /// </summary>
    /// <returns>Новые коды восстановления для пользователя. Примечание. Возвращенное число может быть меньше, поскольку дубликаты будут удалены.</returns>
    public async Task<TResponseModel<IEnumerable<string>?>?> ResponseHandleAction(GenerateNewTwoFactorRecoveryCodesRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.GenerateNewTwoFactorRecoveryCodes(req);
    }
}