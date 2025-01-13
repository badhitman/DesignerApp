////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Get claims
/// </summary>
public class GetClaimsReceive(IIdentityTools idRepo)
    : IResponseReceive<ClaimAreaOwnerModel?, List<ClaimBaseModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetClaimsReceive;

    /// <summary>
    /// Get claims
    /// </summary>
    public async Task<List<ClaimBaseModel>?> ResponseHandleAction(ClaimAreaOwnerModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);        
        return await idRepo.GetClaims(req);
    }
}