////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Пытается удалить предоставленную внешнюю информацию для входа из указанного userId
/// и возвращает флаг, указывающий, удалось ли удаление или нет
/// </summary>
public class RemoveLoginReceive(IIdentityTools idRepo, ILogger<RemoveLoginReceive> loggerRepo)
    : IResponseReceive<RemoveLoginRequestModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RemoveLoginForUserReceive;

    /// <summary>
    /// Пытается удалить предоставленную внешнюю информацию для входа из указанного userId
    /// и возвращает флаг, указывающий, удалось ли удаление или нет
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(RemoveLoginRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.RemoveLoginForUser(req);
    }
}