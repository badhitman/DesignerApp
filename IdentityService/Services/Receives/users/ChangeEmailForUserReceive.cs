////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Обновляет адрес Email, если токен действительный для пользователя.
/// </summary>
public class ChangeEmailForUserReceive(IIdentityTools idRepo, ILogger<ChangeEmailForUserReceive> loggerRepo)
    : IResponseReceive<IdentityEmailTokenModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ChangeEmailForUserReceive;

    /// <summary>
    /// Обновляет адрес Email, если токен действительный для пользователя.
    /// </summary>
    /// <param name="req">Пользователь, адрес электронной почты которого необходимо обновить.Новый адрес электронной почты.Измененный токен электронной почты, который необходимо подтвердить.</param>
    public async Task<ResponseBaseModel?> ResponseHandleAction(IdentityEmailTokenModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.ChangeEmail(req);
    }
}