////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Получает флаг, указывающий, есть ли у пользователя пароль
/// </summary>
public class UserHasPasswordReceive(IIdentityTools idRepo)
    : IResponseReceive<string?, TResponseModel<bool?>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UserHasPasswordReceive;

    /// <summary>
    /// Получает флаг, указывающий, есть ли у пользователя пароль
    /// </summary>
    public async Task<TResponseModel<bool?>?> ResponseHandleAction(string? userId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        return await idRepo.UserHasPassword(userId);
    }
}