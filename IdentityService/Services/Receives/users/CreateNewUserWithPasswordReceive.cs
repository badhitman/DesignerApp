////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Регистрация нового пользователя с паролем (Identity)
/// </summary>
public class CreateNewUserWithPasswordReceive(IIdentityTools idRepo, ILogger<CreateNewUserWithPasswordReceive> loggerRepo)
    : IResponseReceive<RegisterNewUserPasswordModel?, RegistrationNewUserResponseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RegistrationNewUserWithPasswordReceive;

    /// <inheritdoc/>
    public async Task<RegistrationNewUserResponseModel?> ResponseHandleAction(RegisterNewUserPasswordModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.CreateNewUserWithPasswordAsync(req);
    }
}