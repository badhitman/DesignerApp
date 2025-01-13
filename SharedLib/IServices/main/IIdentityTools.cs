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
    /// SetRoleForUser
    /// </summary>
    public Task<TResponseModel<string[]>> SetRoleForUser(SetRoleFoeUserRequestModel req);

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
    /// Find user identity by telegram - receive
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]>> GetUserIdentityByTelegram(long[] req);

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
    /// Get Role (by id)
    /// </summary>
    public Task<TResponseModel<RoleInfoModel>> GetRole(string role_id);

    /// <summary>
    /// Пользователи
    /// </summary>
    public Task<TPaginationResponseModel<UserInfoModel>> FindUsersAsync(FindWithOwnedRequestModel req);

    /// <summary>
    /// Роли. Если указан 'OwnerId', то поиск ограничивается ролями данного пользователя
    /// </summary>
    public Task<TPaginationResponseModel<RoleInfoModel>> FindRolesAsync(FindWithOwnedRequestModel req);

    /// <summary>
    /// Создать новую роль
    /// </summary>
    public Task<ResponseBaseModel> CateNewRole(string role_name);

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
    /// Сбрасывает пароль на указанный
    /// после проверки заданного сброса пароля.
    /// </summary>
    public Task<ResponseBaseModel> ResetPasswordAsync(IdentityPasswordTokenModel req);

    /// <summary>
    /// FindByEmailAsync
    /// </summary>
    public Task<TResponseModel<UserInfoModel>> FindByEmailAsync(string email);

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
    public Task<RegistrationNewUserResponseModel> CreateNewUserEmailAsync(string req);

    /// <summary>
    /// Создать пользователя с паролем
    /// </summary>
    public Task<RegistrationNewUserResponseModel> CreateNewUserWithPasswordAsync(RegisterNewUserPasswordModel req);

    /// <summary>
    /// Установить пользователю Claim`s[TelegramId, FirstName, LastName, PhoneNum]
    /// </summary>
    public Task<TResponseModel<bool>> ClaimsUserFlush(string user_id);

    /// <summary>
    /// Проверяет, соответствует ли токен подтверждения электронной почты указанному пользователю.
    /// </summary>
    /// <param name="req">Пользователь, для которого необходимо проверить токен подтверждения электронной почты.</param>
    public Task<ResponseBaseModel> ConfirmEmailAsync(UserCodeModel req);
}