////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Изменяет пароль пользователя после подтверждения правильности указанного currentPassword.
/// Если userId не указан, то команда выполняется для текущего пользователя (запрос/сессия)
/// </summary>
/// <param name="req">Текущий пароль, который необходимо проверить перед изменением.
/// Новый пароль, который необходимо установить для указанного userId.Пользователь, пароль которого должен быть установлен.
/// Если не указан, то для текущего пользователя (запрос/сессия).</param>
public class ChangePasswordForUserReceive(IIdentityTools idRepo, ILogger<ChangePasswordForUserReceive> loggerRepo)
    : IResponseReceive<IdentityChangePasswordModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ChangePasswordToUserReceive;

    /// <summary>
    /// Изменяет пароль пользователя после подтверждения правильности указанного currentPassword.
    /// Если userId не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    /// <param name="req">Текущий пароль, который необходимо проверить перед изменением.
    /// Новый пароль, который необходимо установить для указанного userId.Пользователь, пароль которого должен быть установлен.
    /// Если не указан, то для текущего пользователя (запрос/сессия).</param>
    public async Task<ResponseBaseModel?> ResponseHandleAction(IdentityChangePasswordModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.ChangePasswordAsync(req);
    }
}