////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Проверяет, соответствует ли токен подтверждения электронной почты указанному пользователю.
/// </summary>
public class ConfirmUserEmailCodeIdentityReceive(IIdentityTools IdentityRepo, ILogger<ConfirmUserEmailCodeIdentityReceive> loggerRepo) 
    : IResponseReceive<UserCodeModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ConfirmUserEmailCodeIdentityReceive;

    /// <summary>
    /// Проверяет, соответствует ли токен подтверждения электронной почты указанному пользователю.
    /// </summary>
    /// <param name="req">Пользователь, для которого необходимо проверить токен подтверждения электронной почты.</param>
    public async Task<ResponseBaseModel?> ResponseHandleAction(UserCodeModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await IdentityRepo.ConfirmEmailAsync(req);
    }
}