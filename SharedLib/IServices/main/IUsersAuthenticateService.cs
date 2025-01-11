﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Сервис работы с аутентификацией пользователей
/// </summary>
public interface IUsersAuthenticateService
{
    /// <summary>
    /// Войти в учётную запись пользователя
    /// </summary>
    public Task<ResponseBaseModel> SignInAsync(string userId, bool isPersistent);

    /// <summary>
    /// Войти в учётную запись пользователя
    /// </summary>
    public Task<IdentityResultResponseModel> PasswordSignInAsync(string userEmail, string password, bool isPersistent);

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    /// <param name="req">Email + Пароль + Адрес сайта/домена (для формирования ссылки подтверждения)</param>
    public Task<RegistrationNewUserResponseModel> RegisterNewUserAsync(RegisterNewUserPasswordModel req);

    /// <summary>
    /// [External] Регистрация нового пользователя
    /// </summary>
    /// <param name="userEmail">Email</param>
    /// <param name="baseAddress">Адрес сайта/домена (для формирования ссылки подтверждения)</param>
    public Task<RegistrationNewUserResponseModel> ExternalRegisterNewUserAsync(string userEmail, string baseAddress);

    /// <summary>
    /// Получает информацию о внешнем входе для текущего входа в виде асинхронной операции.
    /// Gets the external login information for the current login, as an asynchronous operation.
    /// </summary>
    public Task<UserLoginInfoResponseModel> GetExternalLoginInfoAsync(string? expectedXsrf = null);

    /// <summary>
    /// Вход пользователя через ранее зарегистрированный сторонний логин в виде асинхронной операции.
    /// </summary>
    public Task<ExternalLoginSignInResponseModel> ExternalLoginSignInAsync(string loginProvider, string providerKey, string? identityName, bool isPersistent = false, bool bypassTwoFactor = true);

    /// <summary>
    /// Получает информацию о пользователе для текущего входа в систему с двухфакторной аутентификацией.
    /// </summary>
    public Task<TResponseModel<UserInfoModel?>> GetTwoFactorAuthenticationUserAsync();

    /// <summary>
    /// Вход пользователя без двухфакторной аутентификации с использованием двухфакторного кода восстановления.
    /// </summary>
    /// <param name="recoveryCode">Двухфакторный код восстановления.</param>
    public Task<IdentityResultResponseModel> TwoFactorRecoveryCodeSignInAsync(string recoveryCode);

    /// <summary>
    /// Выводит текущего пользователя из приложения.
    /// </summary>
    public Task SignOutAsync();
}