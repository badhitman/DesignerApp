////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Добавляет password к указанному userId, только если у пользователя еще нет пароля.
/// Если userId не указан, то команда выполняется для текущего пользователя (запрос/сессия)
/// </summary>
public class AddPasswordForUserReceive(IIdentityTools idRepo, ILogger<AddPasswordForUserReceive> loggerRepo)
    : IResponseReceive<IdentityPasswordModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddPasswordToUserReceive;

    /// <summary>
    /// Добавляет password к указанному userId, только если у пользователя еще нет пароля.
    /// Если userId не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(IdentityPasswordModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.AddPassword(req);
    }
}