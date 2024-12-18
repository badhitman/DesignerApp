﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Сервис работы с профилями пользователей
/// </summary>
public partial interface IUsersProfilesService
{
    /// <summary>
    /// Get claims
    /// </summary>
    public Task<ClaimBaseModel[]> GetClaims(ClaimAreasEnum claimArea, string ownerId);

    /// <summary>
    /// Claim: Update or create
    /// </summary>
    public Task<ResponseBaseModel> ClaimUpdateOrCreate(ClaimModel claim, ClaimAreasEnum claimArea);

    /// <summary>
    /// Claim: Remove
    /// </summary>
    public Task<ResponseBaseModel> ClaimDelete(ClaimAreasEnum claimArea, int id);

    /// <summary>
    /// Найти пользователя по <paramref name="userId"/>.
    /// Если <paramref name="userId"/> не указан, то возвращается текущий пользователь (запрос/сессия)
    /// </summary>
    public Task<TResponseModel<UserInfoModel?>> FindByIdAsync(string userId);

    /// <summary>
    /// Обновляет адрес Email, если токен <paramref name="token"/> действительный для пользователя <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">Пользователь, адрес электронной почты которого необходимо обновить.</param>
    /// <param name="newEmail">Новый адрес электронной почты.</param>
    /// <param name="token">Измененный токен электронной почты, который необходимо подтвердить.</param>
    public Task<ResponseBaseModel> ChangeEmailAsync(string userId, string newEmail, string token);

    /// <summary>
    /// Получает флаг, указывающий, есть ли у пользователя пароль.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<UserBooleanResponseModel> UserHasPasswordAsync(string? userId = null);

    /// <summary>
    /// Изменяет пароль пользователя после подтверждения правильности указанного <paramref name="currentPassword"/>.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    /// <param name="currentPassword">Текущий пароль, который необходимо проверить перед изменением.</param>
    /// <param name="newPassword">Новый пароль, который необходимо установить для указанного <paramref name="userId"/>.</param>
    /// <param name="userId">Пользователь, пароль которого должен быть установлен. Если не указан, то для текущего пользователя (запрос/сессия)</param>
    public Task<ResponseBaseModel> ChangePasswordAsync(string currentPassword, string newPassword, string? userId = null);

    /// <summary>
    /// Возвращает флаг, указывающий, действителен ли данный <paramref name="password"/> для указанного <paramref name="userId"/>.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    /// <param name="userId">Пользователь, пароль которого необходимо проверить.</param>
    /// <param name="password">Пароль для проверки</param>
    /// <returns>
    /// true, если указанный <paramref name="password" /> соответствует одному хранилищу для <paramref name="userId"/>, в противном случае значение false.
    /// </returns>
    public Task<UserBooleanResponseModel> CheckUserPasswordAsync(string password, string? userId = null);

    /// <summary>
    /// Удалить Identity данные пользователя.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<ResponseBaseModel> DeleteUserDataAsync(string password, string? userId = null);

    /// <summary>
    /// Включена ли для указанного <paramref name="userId"/> двухфакторная аутентификация.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<TResponseModel<bool?>> GetTwoFactorEnabledAsync(string? userId = null);

    /// <summary>
    /// Вкл/Выкл двухфакторную аутентификацию для указанного <paramref name="userId"/>
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<ResponseBaseModel> SetTwoFactorEnabledAsync(bool enabled_set, string? userId = null);

    /// <summary>
    /// Найти пользователя по Email
    /// </summary>
    public Task<UserInfoModel?> FindByEmailAsync(string email);

    /// <summary>
    /// Сбрасывает пароль <paramref name="userId"/> на указанный <paramref name="newPassword"/>
    /// после проверки заданного сброса пароля <paramref name="token"/>.
    /// </summary>
    public Task<ResponseBaseModel> ResetPasswordAsync(string userId, string token, string newPassword);

    /// <summary>
    /// Создать токен подтверждения электронной почты для указанного пользователя.
    /// </summary>
    public Task<ResponseBaseModel> GenerateEmailConfirmationTokenAsync(string userEmail, string baseAddress);

    /// <summary>
    /// Создает токен изменения адреса электронной почты для указанного пользователя.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<ResponseBaseModel> GenerateChangeEmailTokenAsync(string userEmail, string baseAddress, string? userId = null);

    /// <summary>
    /// Добавляет <paramref name="password"/> к указанному <paramref name="userId"/>, только если у пользователя еще нет пароля.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<ResponseBaseModel> AddPasswordAsync(string password, string? userId = null);

    /// <summary>
    /// Был ли подтвержден адрес электронной почты для указанного <paramref name="userId"/>; true, если адрес электронной почты проверен/подтвержден.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<UserBooleanResponseModel> IsEmailConfirmedAsync(string? userId = null);

    /// <summary>
    /// Сбрасывает ключ аутентификации для пользователя.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<ResponseBaseModel> ResetAuthenticatorKeyAsync(string? userId = null);

    /// <summary>
    /// Получает имя пользователя для указанного <paramref name="userId"/>.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<TResponseModel<string?>> GetUserNameAsync(string? userId = null);

    /// <summary>
    /// Получает номер телефона, если таковой имеется, для указанного <paramref name="userId"/>.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<TResponseModel<string?>> GetPhoneNumberAsync(string? userId = null);

    /// <summary>
    /// Устанавливает номер телефона для указанного <paramref name="userId"/>.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<ResponseBaseModel> SetPhoneNumberAsync(string? phoneNumber, string? userId = null);

    /// <summary>
    /// Выполняет вход в указанный <paramref name="userId"/>, сохраняя при этом существующие свойства
    /// AuthenticationProperties текущего вошедшего пользователя, например RememberMe.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<ResponseBaseModel> RefreshSignInAsync(string? userId = null);

    /// <summary>
    /// Извлекает связанные логины для указанного <param ref="userId"/>.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<TResponseModel<IEnumerable<UserLoginInfoModel>?>> GetUserLogins(string? userId = null);

    /// <summary>
    /// Получает хэш пароля для указанного <paramref name="userId"/>.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<TResponseModel<string?>> GetPasswordHashAsync(string? userId = null);

    /// <summary>
    /// Добавляет внешнюю <see cref="UserLoginInfoModel"/> к указанному <paramref name="userId"/>.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<ResponseBaseModel> AddLoginAsync(string? userId = null);

    /// <summary>
    /// Пытается удалить предоставленную внешнюю информацию для входа из указанного <paramref name="userId"/>
    /// и возвращает флаг, указывающий, удалось ли удаление или нет.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<ResponseBaseModel> RemoveLoginAsync(string loginProvider, string providerKey, string? userId = null);

    /// <summary>
    /// Проверяет указанную двухфакторную аутентификацию <paramref name="token" /> на соответствие <paramref name="userId"/>.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<TResponseModel<bool?>> VerifyTwoFactorTokenAsync(string token, string? userId = null);

    /// <summary>
    /// Возвращает количество кодов восстановления, действительных для пользователя.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<TResponseModel<int?>> CountRecoveryCodesAsync(string? userId = null);

    /// <summary>
    /// Генерирует коды восстановления для пользователя, что делает недействительными все предыдущие коды восстановления для пользователя.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    /// <param name="userId">Пользователь, для которого создаются коды восстановления.</param>
    /// <returns>Новые коды восстановления для пользователя. Примечание. Возвращенное число может быть меньше, поскольку дубликаты будут удалены.</returns>
    public Task<TResponseModel<IEnumerable<string>?>> GenerateNewTwoFactorRecoveryCodesAsync(string? userId = null);

    /// <summary>
    /// Ключ аутентификации пользователя.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<TResponseModel<string?>> GetAuthenticatorKeyAsync(string? userId = null);

    /// <summary>
    /// Создает токен сброса пароля для указанного <paramref name="userId"/>, используя настроенного поставщика токенов сброса пароля.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<TResponseModel<string?>> GeneratePasswordResetTokenAsync(string? userId = null);

    /// <summary>
    /// Этот API поддерживает инфраструктуру ASP.NET Core Identity и не предназначен для использования в качестве абстракции электронной почты общего назначения.
    /// Он должен быть реализован в приложении, чтобы инфраструктура идентификации могла отправлять электронные письма для сброса пароля.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<ResponseBaseModel> SendPasswordResetLinkAsync(string email, string baseAddress, string pass_reset_token, string? userId = null);

    /// <summary>
    /// Попытка добавить роль пользователю. Если роли такой нет, то она будет создана.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public Task<ResponseBaseModel> TryAddRolesToUser(IEnumerable<string> addRoles, string? userId = null);

    /// <summary>
    /// Пользователи
    /// </summary>
    public Task<TPaginationStrictResponseModel<UserInfoModel>> FindUsersAsync(FindWithOwnedRequestModel req);

    /// <summary>
    /// Роли. Если указан 'OwnerId', то поиск ограничивается ролями данного пользователя
    /// </summary>
    public Task<TPaginationStrictResponseModel<RoleInfoModel>> FindRolesAsync(FindWithOwnedRequestModel req);

    /// <summary>
    /// Создать новую роль
    /// </summary>
    public Task<ResponseBaseModel> CateNewRole(string role_name);

    /// <summary>
    /// Удалить роль (если у роли нет пользователей).
    /// </summary>
    public Task<ResponseBaseModel> DeleteRole(string role_id);

    /// <summary>
    /// Исключить пользователя из роли (лишить пользователя роли)
    /// </summary>
    public Task<ResponseBaseModel> DeleteRoleFromUser(string role_name, string user_email);

    /// <summary>
    /// Добавить роль пользователю (включить пользователя в роль)
    /// </summary>
    public Task<ResponseBaseModel> AddRoleToUser(string role_name, string user_email);

    /// <summary>
    /// Get Role (by id)
    /// </summary>
    public Task<TResponseModel<RoleInfoModel?>> GetRole(string role_id);

    /// <summary>
    /// Установить блокировку пользователю
    /// </summary>
    public Task<ResponseBaseModel> SetLockUser(string userId, bool locketSet);

    /// <summary>
    /// Обновить пользователю поля: FirstName и LastName
    /// </summary>
    public Task<ResponseBaseModel> UpdateFirstLastNamesUser(string userId, string? firstName, string? lastName, string? phoneNum);
}