////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Удалить роль (если у роли нет пользователей).
/// </summary>
public class DeleteRoleReceive(IIdentityTools idRepo, ILogger<DeleteRoleReceive> loggerRepo)
    : IResponseReceive<string?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.DeleteRoleReceive;

    /// <summary>
    /// Добавить роль пользователю (включить пользователя в роль)
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(string? roleName)
    {
        if(string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentNullException(nameof(roleName));
        
        loggerRepo.LogWarning(JsonConvert.SerializeObject(roleName, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.DeleteRole(roleName);
    }
}