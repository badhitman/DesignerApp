////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using IdentityLib;
using SharedLib;
using IdentityService;

namespace Transmission.Receives.Identity;

/// <summary>
/// Установить пользователю Claim`s[TelegramId, FirstName, LastName, PhoneNum]
/// </summary>
public class ClaimsUserFlushReceive(IdentityTools idRepo)
    : IResponseReceive<string?, TResponseModel<bool>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ClaimsForUserFlushReceive;

    /// <summary>
    /// Установить пользователю Claim`s[TelegramId, FirstName, LastName, PhoneNum]
    /// </summary>
    /// <param name="userId">User id (of Identity)</param>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<TResponseModel<bool>?> ResponseHandleAction(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        return await idRepo.ClaimsUserFlush(userId);
    }
}
