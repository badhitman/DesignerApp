////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Включена ли для указанного userId двухфакторная аутентификация.
/// </summary>
public class GetTwoFactorEnabledReceive(IIdentityTools idRepo)
    : IResponseReceive<string?, TResponseModel<bool?>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetTwoFactorEnabledReceive;

    /// <summary>
    /// Включена ли для указанного <paramref name="userId"/> двухфакторная аутентификация
    /// </summary>
    public async Task<TResponseModel<bool?>?> ResponseHandleAction(string? userId)
    {
        ArgumentNullException.ThrowIfNull(userId);        
        return await idRepo.GetTwoFactorEnabled(userId);
    }
}