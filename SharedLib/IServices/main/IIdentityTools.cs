////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Identity (asp.net)
/// </summary>
public interface IIdentityTools
{
    /// <summary>
    /// Получает флаг, указывающий, есть ли у пользователя пароль
    /// </summary>
    public Task<TResponseModel<bool?>> UserHasPassword(string userId);

    /// <summary>
    /// Включена ли для указанного <paramref name="userId"/> двухфакторная аутентификация.
    /// </summary>
    public Task<TResponseModel<bool?>> GetTwoFactorEnabled(string userId);

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
    public Task<ResponseBaseModel> RemoveLoginForUser(RemoveLoginRequestModel req);

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
    /// </summary>
    public Task<TResponseModel<string?>> GeneratePasswordResetToken(string userId);

    /// <summary>
    /// Этот API поддерживает инфраструктуру ASP.NET Core Identity и не предназначен для использования в качестве абстракции электронной почты общего назначения.
    /// Он должен быть реализован в приложении, чтобы инфраструктура идентификации могла отправлять электронные письма для сброса пароля.
    /// </summary>
    public Task<ResponseBaseModel> SendPasswordResetLink(SendPasswordResetLinkRequestModel req);

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
    /// SelectUsersOfIdentity
    /// </summary>
    public Task<TPaginationResponseModel<UserInfoModel>> SelectUsersOfIdentity(TPaginationRequestModel<SimpleBaseRequestModel> req);

    /// <summary>
    /// Получить пользователей из Identity по их идентификаторам
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]>> GetUsersOfIdentity(string[] req);

    /// <summary>
    /// Получить пользователей из Identity по их Email
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]>> GetUsersIdentityByEmail(string[] req);

    /// <summary>
    /// Обновляет адрес Email, если токен действительный для пользователя.
    /// </summary>
    /// <param name="req">Пользователь, адрес электронной почты которого необходимо обновить.Новый адрес электронной почты.Измененный токен электронной почты, который необходимо подтвердить.</param>
    public Task<ResponseBaseModel> ChangeEmail(IdentityEmailTokenModel req);

    /// <summary>
    /// Обновить пользователю поля: FirstName и LastName
    /// </summary>
    public Task<ResponseBaseModel> UpdateUserDetails(IdentityDetailsModel req);

    /// <summary>
    /// Установить блокировку пользователю
    /// </summary>
    public Task<ResponseBaseModel> SetLockUser(IdentityBooleanModel req);

    /// <summary>
    /// Пользователи
    /// </summary>
    public Task<TPaginationResponseModel<UserInfoModel>> FindUsers(FindWithOwnedRequestModel req);

    /// <summary>
    /// Сбрасывает пароль на указанный
    /// после проверки заданного сброса пароля.
    /// </summary>
    public Task<ResponseBaseModel> ResetPassword(IdentityPasswordTokenModel req);

    /// <summary>
    /// FindByEmail
    /// </summary>
    public Task<TResponseModel<UserInfoModel>> FindByEmail(string email);

    /// <summary>
    /// Создает и отправляет токен подтверждения электронной почты для указанного пользователя.
    /// </summary>
    /// <remarks>
    /// Этот API поддерживает инфраструктуру ASP.NET Core Identity и не предназначен для использования в качестве абстракции электронной почты общего назначения.
    /// Он должен быть реализован в приложении, чтобы  Identityинфраструктура могла отправлять электронные письма с подтверждением.
    /// </remarks>
    public Task<ResponseBaseModel> GenerateEmailConfirmation(SimpleUserIdentityModel req);

    /// <summary>
    /// Создать пользователя (без пароля)
    /// </summary>
    public Task<RegistrationNewUserResponseModel> CreateNewUserEmail(string req);

    /// <summary>
    /// Создать пользователя с паролем
    /// </summary>
    public Task<RegistrationNewUserResponseModel> CreateNewUserWithPassword(RegisterNewUserPasswordModel req);

    /// <summary>
    /// Проверяет, соответствует ли токен подтверждения электронной почты указанному пользователю.
    /// </summary>
    /// <param name="req">Пользователь, для которого необходимо проверить токен подтверждения электронной почты.</param>
    public Task<ResponseBaseModel> ConfirmEmail(UserCodeModel req);

    #region claims
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
    /// Установить пользователю Claim`s[TelegramId, FirstName, LastName, PhoneNum]
    /// </summary>
    public Task<TResponseModel<bool>> ClaimsUserFlush(string user_id);
    #endregion

    #region telegram
    /// <summary>
    /// Find user identity by telegram - receive
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]>> GetUsersIdentityByTelegram(List<long> req);

    /// <summary>
    /// Инициировать новую процедуру привязки Telegram аккаунта к учётной записи сайта
    /// </summary>
    public Task<TResponseModel<TelegramJoinAccountModelDb>> TelegramJoinAccountCreate(string userId);

    /// <summary>
    /// Удалить связь Telegram аккаунта с учётной записью сайта
    /// </summary>
    public Task<ResponseBaseModel> TelegramAccountRemoveIdentityJoin(TelegramAccountRemoveJoinRequestIdentityModel req);

    /// <summary>
    /// Удалить текущую процедуру привязки Telegram аккаунта к учётной записи сайта
    /// </summary>
    public Task<ResponseBaseModel> TelegramJoinAccountDeleteAction(string userId);

    /// <summary>
    /// Telegram пользователи (сохранённые).
    /// Все пользователи, которые когда либо писали что либо в бота - сохраняются/кэшируются в БД.
    /// </summary>
    public Task<TPaginationResponseModel<TelegramUserViewModel>> FindUsersTelegram(FindRequestModel req);

    /// <summary>
    /// Telegram: Подтверждение токена
    /// </summary>
    public Task<ResponseBaseModel> TelegramJoinAccountConfirmTokenFromTelegram(TelegramJoinAccountConfirmModel req);

    /// <summary>
    /// Получить информацию по пользователю (из БД).
    /// Данные возвращаются из кэша: каждое сообщение в TelegramBot кеширует информацию о пользователе в БД
    /// </summary>
    public Task<TResponseModel<TelegramUserBaseModel>> GetTelegramUserCachedInfo(long telegramId);

    /// <summary>
    /// Удалить связь Telegram аккаунта с учётной записью сайта
    /// </summary>
    public Task<ResponseBaseModel> TelegramAccountRemoveTelegramJoin(TelegramAccountRemoveJoinRequestTelegramModel req);

    /// <summary>
    /// Установить/обновить основное сообщение в чате в котором Bot ведёт диалог с пользователем.
    /// Бот может отвечать новым сообщением или редактировать своё ранее отправленное в зависимости от ситуации.
    /// </summary>
    public Task<ResponseBaseModel> UpdateTelegramMainUserMessage(MainUserMessageModel setMainUserMessage);

    /// <summary>
    /// Получить состояние процедуры привязки аккаунта Telegram к учётной записи сайта (если есть).
    /// Если userId не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<TResponseModel<TelegramJoinAccountModelDb>> TelegramJoinAccountState(TelegramJoinAccountStateRequestModel req);

    /// <summary>
    /// Проверка пользователя (сообщение из службы TelegramBot серверной части сайта)
    /// </summary>
    public Task<TResponseModel<CheckTelegramUserAuthModel>> CheckTelegramUser(CheckTelegramUserHandleModel user);
    #endregion

    #region roles
    /// <summary>
    /// Попытка добавить роли пользователю. Если роли такой нет, то она будет создана.
    /// </summary>
    public Task<ResponseBaseModel> TryAddRolesToUser(UserRolesModel req);

    /// <summary>
    /// SetRoleForUser
    /// </summary>
    public Task<TResponseModel<string[]>> SetRoleForUser(SetRoleForUserRequestModel req);

    /// <summary>
    /// Get Role (by id)
    /// </summary>
    public Task<TResponseModel<RoleInfoModel>> GetRole(string role_id);

    /// <summary>
    /// Роли. Если указан 'OwnerId', то поиск ограничивается ролями данного пользователя
    /// </summary>
    public Task<TPaginationResponseModel<RoleInfoModel>> FindRoles(FindWithOwnedRequestModel req);

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
    #endregion
}