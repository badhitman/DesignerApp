////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Сбрасывает ключ аутентификации для пользователя.
/// </summary>
public class ResetAuthenticatorKeyReceive(IIdentityTools idRepo, ILogger<ResetAuthenticatorKeyReceive> loggerRepo)
    : IResponseReceive<string?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ResetAuthenticatorKeyReceive;

    /// <summary>
    /// Сбрасывает ключ аутентификации для пользователя.
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(string? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.ResetAuthenticatorKey(req);
    }
}