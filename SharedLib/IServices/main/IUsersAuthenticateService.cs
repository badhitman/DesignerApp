////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Сервис работы с аутентификацией пользователей
/// </summary>
public interface IUsersAuthenticateService
{
    /// <summary>
    /// Проверяет код входа из приложения проверки подлинности, а также создает и подписывает пользователя в виде асинхронной операции.
    /// </summary>
    /// <param name="code">Код двухфакторной аутентификации для проверки.</param>
    /// <param name="isPersistent">Флаг, указывающий, должен ли файл cookie для входа сохраняться после закрытия браузера.</param>
    /// <param name="rememberClient">Флаг, указывающий, следует ли запомнить текущий браузер, подавляя все дальнейшие запросы двухфакторной аутентификации.</param>
    public Task<IdentityResultResponseModel> TwoFactorAuthenticatorSignIn(string code, bool isPersistent, bool rememberClient);

    /// <summary>
    /// Войти в учётную запись пользователя
    /// </summary>
    public Task<ResponseBaseModel> SignIn(string userId, bool isPersistent);

    /// <summary>
    /// Войти в учётную запись пользователя
    /// </summary>
    public Task<SignInResultResponseModel> PasswordSignIn(string userEmail, string password, bool isPersistent);

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    /// <param name="req">Email + Пароль + Адрес сайта/домена (для формирования ссылки подтверждения)</param>
    public Task<RegistrationNewUserResponseModel> RegisterNewUser(RegisterNewUserPasswordModel req);

    /// <summary>
    /// [External] Регистрация нового пользователя
    /// </summary>
    /// <param name="userEmail">Email</param>
    /// <param name="baseAddress">Адрес сайта/домена (для формирования ссылки подтверждения)</param>
    public Task<RegistrationNewUserResponseModel> ExternalRegisterNewUser(string userEmail, string baseAddress);

    /// <summary>
    /// Получает информацию о внешнем входе для текущего входа в виде асинхронной операции.
    /// Gets the external login information for the current login, as an asynchronous operation.
    /// </summary>
    public Task<UserLoginInfoResponseModel> GetExternalLoginInfo(string? expectedXsrf = null);

    /// <summary>
    /// Вход пользователя через ранее зарегистрированный сторонний логин в виде асинхронной операции.
    /// </summary>
    public Task<ExternalLoginSignInResponseModel> ExternalLoginSignIn(string loginProvider, string providerKey, string? identityName, bool isPersistent = false, bool bypassTwoFactor = true);

    /// <summary>
    /// Получает информацию о пользователе для текущего входа в систему с двухфакторной аутентификацией.
    /// </summary>
    public Task<TResponseModel<UserInfoModel?>> GetTwoFactorAuthenticationUser();

    /// <summary>
    /// Вход пользователя без двухфакторной аутентификации с использованием двухфакторного кода восстановления.
    /// </summary>
    /// <param name="recoveryCode">Двухфакторный код восстановления.</param>
    public Task<IdentityResultResponseModel> TwoFactorRecoveryCodeSignIn(string recoveryCode);

    /// <summary>
    /// Выводит текущего пользователя из приложения.
    /// </summary>
    public Task SignOut();
}