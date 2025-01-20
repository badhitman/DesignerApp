////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Создает токен сброса пароля для указанного "userId", используя настроенного поставщика токенов сброса пароля.
/// Если "userId" не указан, то команда выполняется для текущего пользователя (запрос/сессия)
/// </summary>
public class GeneratePasswordResetTokenReceive(IIdentityTools idRepo)
    : IResponseReceive<string?, TResponseModel<string?>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GeneratePasswordResetTokenReceive;

    /// <summary>
    /// Создает токен сброса пароля для указанного "userId", используя настроенного поставщика токенов сброса пароля.
    /// Если "userId" не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public async Task<TResponseModel<string?>?> ResponseHandleAction(string? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await idRepo.GeneratePasswordResetTokenAsync(req);
    }
}