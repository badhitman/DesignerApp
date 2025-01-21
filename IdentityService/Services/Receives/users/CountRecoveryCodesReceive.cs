////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Возвращает количество кодов восстановления, действительных для пользователя
/// </summary>
public class CountRecoveryCodesReceive(IIdentityTools idRepo, ILogger<CountRecoveryCodesReceive> loggerRepo)
    : IResponseReceive<string?, TResponseModel<int?>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CountRecoveryCodesReceive;

    /// <summary>
    /// Возвращает количество кодов восстановления, действительных для пользователя
    /// </summary>
    public async Task<TResponseModel<int?>?> ResponseHandleAction(string? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.CountRecoveryCodes(req);
    }
}