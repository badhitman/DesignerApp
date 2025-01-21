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
    /// Вкл/Выкл двухфакторную аутентификацию для указанного userId
    /// </summary>
    public Task<ResponseBaseModel> SetTwoFactorEnabled(SetTwoFactorEnabledRequestModel req);

    /// <summary>
    /// Сбрасывает ключ аутентификации для пользователя.
    /// </summary>
    public Task<ResponseBaseModel> ResetAuthenticatorKey(string userId);

    /// <summary>
    /// Пытается удалить предоставленную внешнюю информацию для входа из указанного userId
    /// и возвращает флаг, указывающий, удалось ли удаление или нет
    /// </summary>
    public Task<ResponseBaseModel> RemoveLogin(RemoveLoginRequestModel req);

    /// <summary>
    /// Проверяет указанную двухфакторную аутентификацию VerificationCode на соответствие UserId
    /// </summary>
    public Task<ResponseBaseModel> VerifyTwoFactorToken(VerifyTwoFactorTokenRequestModel req);

    /// <summary>
    /// Возвращает количество кодов восстановления, действительных для пользователя
    /// </summary>
    public Task<TResponseModel<int?>> CountRecoveryCodes(string userId);

    /// <summary>
    /// Создает (и отправляет) токен изменения адреса электронной почты для указанного пользователя.
    /// </summary>
    public Task<ResponseBaseModel> GenerateChangeEmailToken(GenerateChangeEmailTokenRequestModel req);

    /// <summary>
    /// Генерирует коды восстановления для пользователя, что делает недействительными все предыдущие коды восстановления для пользователя.
    /// </summary>
    /// <returns>Новые коды восстановления для пользователя. Примечание. Возвращенное число может быть меньше, поскольку дубликаты будут удалены.</returns>
    public Task<TResponseModel<IEnumerable<string>?>> GenerateNewTwoFactorRecoveryCodes(GenerateNewTwoFactorRecoveryCodesRequestModel req);

    /// <summary>
    /// Ключ аутентификации пользователя.
    /// </summary>
    public Task<TResponseModel<string?>> GetAuthenticatorKey(string userId);

    /// <summary>
    /// Создает токен сброса пароля для указанного <paramref name="userId"/>, используя настроенного поставщика токенов сброса пароля.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<TResponseModel<string?>> GeneratePasswordResetTokenAsync(string userId);

    /// <summary>
    /// Этот API поддерживает инфраструктуру ASP.NET Core Identity и не предназначен для использования в качестве абстракции электронной почты общего назначения.
    /// Он должен быть реализован в приложении, чтобы инфраструктура идентификации могла отправлять электронные письма для сброса пароля.
    /// </summary>
    public Task<ResponseBaseModel> SendPasswordResetLinkAsync(SendPasswordResetLinkRequestModel req);

    /// <summary>
    /// Изменяет пароль пользователя после подтверждения правильности указанного currentPassword.
    /// Если userId не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    /// <param name="req">Текущий пароль, который необходимо проверить перед изменением.
    /// Новый пароль, который необходимо установить для указанного userId.Пользователь, пароль которого должен быть установлен.
    /// Если не указан, то для текущего пользователя (запрос/сессия).</param>
    public Task<ResponseBaseModel> ChangePassword(IdentityChangePasswordModel req);

    /// <summary>
    /// Добавляет password к указанному userId, только если у пользователя еще нет пароля.
    /// Если userId не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<ResponseBaseModel> AddPassword(IdentityPasswordModel req);

    /// <summary>
    /// Обновляет адрес Email, если токен действительный для пользователя.
    /// </summary>
    /// <param name="req">Пользователь, адрес электронной почты которого необходимо обновить.Новый адрес электронной почты.Измененный токен электронной почты, который необходимо подтвердить.</param>
    public Task<ResponseBaseModel> ChangeEmailAsync(IdentityEmailTokenModel req);

    /// <summary>
    /// Обновить пользователю поля: FirstName и LastName
    /// </summary>
    public Task<ResponseBaseModel> UpdateUserDetails(IdentityDetailsModel req);

    /// <summary>
    /// Claim: Remove
    /// </summary>
    public Task<ResponseBaseModel> ClaimDelete(ClaimAreaIdModel req);

    /// <summary>
    /// Claim: Update or create
    /// </summary>
    public Task<ResponseBaseModel> ClaimUpdateOrCreate(ClaimUpdateModel req);

    /// <summary>
    /// Get claims
    /// </summary>
    public Task<List<ClaimBaseModel>> GetClaims(ClaimAreaOwnerModel req);

    /// <summary>
    /// Установить блокировку пользователю
    /// </summary>
    public Task<ResponseBaseModel> SetLockUser(IdentityBooleanModel req);

    /// <summary>
    /// Пользователи
    /// </summary>
    public Task<TPaginationResponseModel<UserInfoModel>> FindUsersAsync(FindWithOwnedRequestModel req);

    /// <summary>
    /// Сбрасывает пароль на указанный
    /// после проверки заданного сброса пароля.
    /// </summary>
    public Task<ResponseBaseModel> ResetPassword(IdentityPasswordTokenModel req);

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
    /// Получить пользователей из Identity по их Email`s
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]>> GetUsersIdentityByEmails(IEnumerable<string> ids_emails);

    #region roles
    /// <summary>
    /// Попытка добавить роли пользователю. Если роли такой нет, то она будет создана.
    /// </summary>
    public Task<ResponseBaseModel> TryAddRolesToUser(UserRolesModel req);

    /// <summary>
    /// Get Role (by id)
    /// </summary>
    public Task<TResponseModel<RoleInfoModel>> GetRole(string roleName);

    /// <summary>
    /// Роли. Если указан 'OwnerId', то поиск ограничивается ролями данного пользователя
    /// </summary>
    public Task<TPaginationResponseModel<RoleInfoModel>> FindRolesAsync(FindWithOwnedRequestModel req);

    /// <summary>
    /// Создать новую роль
    /// </summary>
    public Task<ResponseBaseModel> CreateNewRole(string role_name);

    /// <summary>
    /// Удалить роль (если у роли нет пользователей).
    /// </summary>
    public Task<ResponseBaseModel> DeleteRole(string roleName);

    /// <summary>
    /// Исключить пользователя из роли (лишить пользователя роли)
    /// </summary>
    public Task<ResponseBaseModel> DeleteRoleFromUser(RoleEmailModel req);

    /// <summary>
    /// Добавить роль пользователю (включить пользователя в роль)
    /// </summary>
    public Task<ResponseBaseModel> AddRoleToUser(RoleEmailModel req);

    /// <summary>
    /// Получить `web config` сайта
    /// </summary>
    public Task<TResponseModel<string[]>> SetRoleForUser(SetRoleForUserRequestModel req);
    #endregion

    #region tg
    /// <summary>
    /// Установить пользователю Claim`s[TelegramId, FirstName, LastName, PhoneNum]
    /// </summary>
    public Task<TResponseModel<bool>> ClaimsUserFlush(string userIdIdentity);

    /// <summary>
    /// Поиск пользователей в Identity по их Telegram chat id
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]>> GetUserIdentityByTelegram(long[] ids_users);

    /// <summary>
    /// Find user identity by telegram - receive
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]>> GetUsersIdentityByTelegram(List<long> req);

    /// <summary>
    /// Удалить связь Telegram аккаунта с учётной записью сайта
    /// </summary>
    public Task<ResponseBaseModel> TelegramAccountRemoveIdentityJoin(TelegramAccountRemoveJoinRequestIdentityModel req);

    /// <summary>
    /// Удалить текущую процедуру привязки Telegram аккаунта к учётной записи сайта
    /// </summary>
    public Task<ResponseBaseModel> TelegramJoinAccountDeleteAction(string userId);

    /// <summary>
    /// Инициировать новую процедуру привязки Telegram аккаунта к учётной записи сайта
    /// </summary>
    public Task<TResponseModel<TelegramJoinAccountModelDb>> TelegramJoinAccountCreate(string userId);

    /// <summary>
    /// Telegram пользователи (сохранённые).
    /// Все пользователи, которые когда либо писали что либо в бота - сохраняются/кэшируются в БД.
    /// </summary>
    public Task<TPaginationResponseModel<TelegramUserViewModel>> FindUsersTelegram(FindRequestModel req);

    /// <summary>
    /// Проверка Telegram пользователя
    /// </summary>
    public Task<ResponseBaseModel> TelegramJoinAccountConfirmToken(TelegramJoinAccountConfirmModel req, bool waitResponse = true);

    /// <summary>
    /// Удалить связь Telegram аккаунта с учётной записью сайта
    /// </summary>
    public Task<ResponseBaseModel> TelegramJoinAccountDelete(TelegramAccountRemoveJoinRequestTelegramModel req);

    /// <summary>
    /// Основное сообщение в чате в котором Bot ведёт диалог с пользователем.
    /// Бот может отвечать новым сообщением или редактировать своё ранее отправленное в зависимости от ситуации.
    /// </summary>
    public Task<ResponseBaseModel> UpdateTelegramMainUserMessage(MainUserMessageModel setMainMessage);

    /// <summary>
    /// Получить данные пользователя из кэша
    /// </summary>
    public Task<TResponseModel<TelegramUserBaseModel>> GetTelegramUser(long telegramUserId);

    /// <summary>
    /// TelegramJoinAccountState
    /// </summary>
    public Task<TResponseModel<TelegramJoinAccountModelDb>> TelegramJoinAccountState(TelegramJoinAccountStateRequestModel req);

    /// <summary>
    /// Проверка пользователя (сообщение из службы TelegramBot серверной части сайта)
    /// </summary>
    public Task<TResponseModel<CheckTelegramUserAuthModel>> CheckTelegramUser(CheckTelegramUserHandleModel user);
    #endregion
}