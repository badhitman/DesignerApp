////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Создать новую роль
/// </summary>
public class CateNewRoleReceive(IIdentityTools idRepo, ILogger<CateNewRoleReceive> loggerRepo)
    : IResponseReceive<string?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CateNewRoleReceive;

    /// <summary>
    /// Создать новую роль
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(string? roleName)
    {
        if(string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentNullException(nameof(roleName));

        loggerRepo.LogWarning(JsonConvert.SerializeObject(roleName, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.CreateNewRole(roleName);
    }
}