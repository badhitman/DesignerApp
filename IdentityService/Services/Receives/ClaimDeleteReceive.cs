////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Claim: Remove
/// </summary>
public class ClaimDeleteReceive(IIdentityTools idRepo, ILogger<ClaimDeleteReceive> loggerRepo)
    : IResponseReceive<ClaimAreaIdModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ClaimDeleteReceive;

    /// <summary>
    /// Claim: Remove
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(ClaimAreaIdModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogWarning(JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings));
        return await idRepo.ClaimDelete(req);
    }
}