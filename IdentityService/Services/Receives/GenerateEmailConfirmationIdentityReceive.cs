////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Создает и отправляет токен подтверждения электронной почты для указанного пользователя.
/// </summary>
/// <remarks>
/// Этот API поддерживает инфраструктуру ASP.NET Core Identity и не предназначен для использования в качестве абстракции электронной почты общего назначения.
/// Он должен быть реализован в приложении, чтобы  Identityинфраструктура могла отправлять электронные письма с подтверждением.
/// </remarks>
public class GenerateEmailConfirmationIdentityReceive(IIdentityTools IdentityRepo)
    : IResponseReceive<SimpleUserIdentityModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GenerateEmailConfirmationIdentityReceive;

    /// <summary>
    /// Создает и отправляет токен подтверждения электронной почты для указанного пользователя.
    /// </summary>
    /// <remarks>
    /// Этот API поддерживает инфраструктуру ASP.NET Core Identity и не предназначен для использования в качестве абстракции электронной почты общего назначения.
    /// Он должен быть реализован в приложении, чтобы  Identityинфраструктура могла отправлять электронные письма с подтверждением.
    /// </remarks>
    public async Task<ResponseBaseModel?> ResponseHandleAction(SimpleUserIdentityModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await IdentityRepo.GenerateEmailConfirmation(req);
    }
}