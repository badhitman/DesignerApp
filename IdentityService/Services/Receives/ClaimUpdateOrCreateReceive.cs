////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Claim: Update or create
/// </summary>
public class ClaimUpdateOrCreateReceive(IIdentityTools idRepo, ILogger<ClaimUpdateOrCreateReceive> loggerRepo)
    : IResponseReceive<ClaimUpdateModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ClaimUpdateOrCreateReceive;

    /// <summary>
    /// Claim: Update or create
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(ClaimUpdateModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.ClaimUpdateOrCreate(req);
    }
}