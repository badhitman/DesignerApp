////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Проверяет, соответствует ли токен подтверждения электронной почты указанному пользователю.
/// </summary>
public class ConfirmUserEmailCodeIdentityReceive(IIdentityTools IdentityRepo) 
    : IResponseReceive<UserCodeModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ConfirmUserEmailCodeIdentityReceive;

    /// <summary>
    /// Проверяет, соответствует ли токен подтверждения электронной почты указанному пользователю.
    /// </summary>
    /// <param name="req">Пользователь, для которого необходимо проверить токен подтверждения электронной почты.</param>
    public async Task<ResponseBaseModel?> ResponseHandleAction(UserCodeModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await IdentityRepo.ConfirmEmailAsync(req);
    }
}