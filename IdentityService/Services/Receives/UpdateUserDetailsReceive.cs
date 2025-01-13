////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Обновить пользователю поля: FirstName и LastName
/// </summary>
public class UpdateUserDetailsReceive(IIdentityTools idRepo, ILogger<UpdateUserDetailsReceive> loggerRepo)
    : IResponseReceive<IdentityDetailsModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateUserDetailsReceive;

    /// <summary>
    /// Обновить пользователю поля: FirstName и LastName
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(IdentityDetailsModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.UpdateUserDetails(req);
    }
}