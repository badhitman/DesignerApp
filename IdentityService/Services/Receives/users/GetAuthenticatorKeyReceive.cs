////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Ключ аутентификации пользователя.
/// </summary>
public class GetAuthenticatorKeyReceive(IIdentityTools idRepo)
    : IResponseReceive<string?, TResponseModel<string?>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetAuthenticatorKeyReceive;

    /// <summary>
    /// Ключ аутентификации пользователя.
    /// </summary>
    public async Task<TResponseModel<string?>?> ResponseHandleAction(string? req)
    {
        ArgumentNullException.ThrowIfNull(req);        
        return await idRepo.GetAuthenticatorKey(req);
    }
}