////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Извлекает связанные логины для указанного <param ref="userId"/>
/// </summary>
public class GetUserLoginsReceive(IIdentityTools idRepo, ILogger<AddPasswordForUserReceive> loggerRepo)
    : IResponseReceive<string?, TResponseModel<IEnumerable<UserLoginInfoModel>>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetUserLoginsReceive;

    /// <summary>
    /// Извлекает связанные логины для указанного <param ref="userId"/>
    /// </summary>
    public async Task<TResponseModel<IEnumerable<UserLoginInfoModel>>?> ResponseHandleAction(string? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.GetUserLogins(req);
    }
}