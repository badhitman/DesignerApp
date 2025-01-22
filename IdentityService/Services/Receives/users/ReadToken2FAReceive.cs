////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Чтение 2fa токена (из кеша)
/// </summary>
public class ReadToken2FAReceive(IIdentityTools idRepo)
    : IResponseReceive<string?, TResponseModel<string>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadToken2FAReceive;

    /// <summary>
    /// Чтение 2fa токена (из кеша)
    /// </summary>
    public async Task<TResponseModel<string>?> ResponseHandleAction(string? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await idRepo.ReadToken2FA(req);
    }
}