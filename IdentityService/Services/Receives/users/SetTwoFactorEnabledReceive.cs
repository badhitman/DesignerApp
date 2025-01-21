////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Вкл/Выкл двухфакторную аутентификацию для указанного userId
/// </summary>
public class SetTwoFactorEnabledReceive(IIdentityTools idRepo, ILogger<SetTwoFactorEnabledReceive> loggerRepo)
    : IResponseReceive<SetTwoFactorEnabledRequestModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetTwoFactorEnabledReceive;

    /// <summary>
    /// Вкл/Выкл двухфакторную аутентификацию для указанного userId
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(SetTwoFactorEnabledRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.SetTwoFactorEnabled(req);
    }
}