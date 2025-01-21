////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Изменяет пароль пользователя после подтверждения правильности указанного currentPassword
/// </summary>
public class ChangePasswordForUserReceive(IIdentityTools idRepo, ILogger<ChangePasswordForUserReceive> loggerRepo)
    : IResponseReceive<IdentityChangePasswordModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ChangePasswordToUserReceive;

    /// <summary>
    /// Изменяет пароль пользователя после подтверждения правильности указанного currentPassword
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(IdentityChangePasswordModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.ChangePassword(req);
    }
}