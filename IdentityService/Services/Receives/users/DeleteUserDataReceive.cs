////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Удалить Identity данные пользователя
/// </summary>
public class DeleteUserDataReceive(IIdentityTools idRepo, ILogger<AddPasswordForUserReceive> loggerRepo)
    : IResponseReceive<DeleteUserDataRequestModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.DeleteUserDataReceive;

    /// <summary>
    /// Удалить Identity данные пользователя
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(DeleteUserDataRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.DeleteUserData(req);
    }
}