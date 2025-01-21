////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace IdentityService.Services.Receives.users;

/// <summary>
/// Сбрасывает пароль на указанный
/// после проверки заданного сброса пароля.
/// </summary>
public class ResetPasswordReceive(IIdentityTools idRepo, ILogger<ResetPasswordReceive> loggerRepo)
    : IResponseReceive<IdentityPasswordTokenModel?, ResponseBaseModel?>
{
    /// <summary>
    /// Сбрасывает пароль на указанный
    /// после проверки заданного сброса пароля.
    /// </summary>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ResetPasswordForUserReceive;

    /// <summary>
    /// Сбрасывает пароль на указанный
    /// после проверки заданного сброса пароля.
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(IdentityPasswordTokenModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.ResetPassword(req);
    }
}