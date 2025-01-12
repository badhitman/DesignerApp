////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Identity
/// </summary>
public interface IIdentityTransmission
{
    /// <summary>
    /// Поиск пользователя по Email
    /// </summary>
    public Task<TResponseModel<UserInfoModel>> FindUserByEmail(string email);

    /// <summary>
    /// Создает и отправляет токен подтверждения электронной почты для указанного пользователя.
    /// </summary>
    /// <remarks>
    /// Этот API поддерживает инфраструктуру ASP.NET Core Identity и не предназначен для использования в качестве абстракции электронной почты общего назначения.
    /// Он должен быть реализован в приложении, чтобы  Identityинфраструктура могла отправлять электронные письма с подтверждением.
    /// </remarks>
    public Task<ResponseBaseModel> GenerateEmailConfirmation(SimpleUserIdentityModel req);

    /// <summary>
    /// Регистрация нового email/пользователя
    /// </summary>
    /// <remarks>
    /// Без пароля
    /// </remarks>
    public Task<RegistrationNewUserResponseModel> CreateNewUser(string userEmail);

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    /// <param name="req">Email + Пароль + Адрес сайта/домена (для формирования ссылки подтверждения)</param>
    public Task<RegistrationNewUserResponseModel> CreateNewUserWithPassword(RegisterNewUserPasswordModel req);

    /// <summary>
    /// Проверяет, соответствует ли токен подтверждения электронной почты указанному пользователю.
    /// </summary>
    /// <param name="req">Пользователь, для которого необходимо проверить токен подтверждения электронной почты.</param>
    public Task<ResponseBaseModel> ConfirmUserEmailCode(UserCodeModel req);

    /// <summary>
    /// Получить `web config` сайта
    /// </summary>
    public Task<TResponseModel<string[]>> SetRoleForUser(SetRoleFoeUserRequestModel req);

    /// <summary>
    /// Отправка Email
    /// </summary>
    public Task<ResponseBaseModel> SendEmail(SendEmailRequestModel req, bool waitResponse = true);

    /// <summary>
    /// SelectUsersOfIdentity
    /// </summary>
    public Task<TPaginationResponseModel<UserInfoModel>> SelectUsersOfIdentity(TPaginationRequestModel<SimpleBaseRequestModel> req);

    /// <summary>
    /// Получить пользователей из Identity по их идентификаторам
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]>> GetUsersIdentity(IEnumerable<string> ids_users);

    /// <summary>
    /// Установить пользователю Claim`s[TelegramId, FirstName, LastName, PhoneNum]
    /// </summary>
    public Task<TResponseModel<bool>> ClaimsUserFlush(string userIdIdentity);

    /// <summary>
    /// Получить пользователей из Identity по их Email`s
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]>> GetUsersIdentityByEmails(IEnumerable<string> ids_emails);

    /// <summary>
    /// Поиск пользователей в Identity по их Telegram chat id
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]>> GetUserIdentityByTelegram(long[] ids_users);

}
