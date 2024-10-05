using IdentityLib;
using Microsoft.AspNetCore.Identity;
using SharedLib;

namespace ServerLib;

/// <summary>
/// Инфраструктура ASP.NET Core Identity (не предназначено для использования в качестве абстракции электронной почты общего назначения).
/// Отправка электронных писем с подтверждением и сбросом пароля.
/// This API supports the ASP.NET Core Identity infrastructure and is not intended to be used as a general purpose email abstraction. It should be implemented by the application so the Identity infrastructure can send confirmation and password reset emails.
/// </summary>
public sealed class IdentityEmailSender(IMailProviderService emailSender) : IEmailSender<ApplicationUser>
{
    /// <inheritdoc/>
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        emailSender.SendEmailAsync(email, "Подтвердите ваш адрес электронной почты", $"Пожалуйста, подтвердите свой аккаунт <a href='{confirmationLink}'>кликнув по ссылке</a>.");
    /// <inheritdoc/>

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        emailSender.SendEmailAsync(email, "Сброс пароля", $"Для сброса пароля - <a href='{resetLink}'>кликните по ссылке</a>.");
    /// <inheritdoc/>

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        emailSender.SendEmailAsync(email, "Сброс пароля", $"Пожалуйста, сбросьте пароль, используя следующий код: {resetCode}");
}