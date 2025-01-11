////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Регистрация нового пользователя (Identity)
/// </summary>
public class CreateNewUserReceive(IIdentityTools idRepo, ILogger<CreateNewUserReceive> loggerRepo)
    : IResponseReceive<RegisterNewUserModel?, RegistrationNewUserResponseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RegistrationNewUserReceive;

    /// <summary>
    /// Установить пользователю Claim`s[TelegramId, FirstName, LastName, PhoneNum]
    /// </summary>
    public async Task<RegistrationNewUserResponseModel?> ResponseHandleAction(RegisterNewUserModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.CreateNewUserAsync(req);
    }
}