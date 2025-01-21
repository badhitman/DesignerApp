////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Генерация (и отправка на Email++) 2fa токена
/// </summary>
public class GenerateOTPFor2StepVerificationReceive(IIdentityTools idRepo, ILogger<AddPasswordForUserReceive> loggerRepo)
    : IResponseReceive<string?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GenerateOTPFor2StepVerificationReceive;

    /// <summary>
    /// Генерация (и отправка на Email++) 2fa токена
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(string? userId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(userId, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.GenerateOTPFor2StepVerification(userId);
    }
}