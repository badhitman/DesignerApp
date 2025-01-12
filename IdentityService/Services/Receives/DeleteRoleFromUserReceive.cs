////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Исключить пользователя из роли (лишить пользователя роли)
/// </summary>
public class DeleteRoleFromUserReceive(IIdentityTools idRepo, ILogger<DeleteRoleFromUserReceive> loggerRepo)
    : IResponseReceive<RoleEmailModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.DeleteRoleFromUserReceive;

    /// <summary>
    /// Исключить пользователя из роли (лишить пользователя роли)
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(RoleEmailModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.DeleteRoleFromUser(req);
    }
}