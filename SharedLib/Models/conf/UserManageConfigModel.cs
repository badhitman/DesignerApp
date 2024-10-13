////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Net.Mail;

namespace SharedLib;

/// <summary>
/// Настройки пользователей системы
/// </summary>
public class UserManageConfigModel
{
    /// <summary>
    /// Запрет регистрации
    /// </summary>
    public DenyActionModel? DenyRegistration { get; set; }

    /// <summary>
    /// Запрет авторизации
    /// </summary>
    public DenyActionModel? DenyAuthorization { get; set; }

    /// <summary>
    /// Установка ролей пользователям по их Email. Процедура запускается при каждом входе пользователя.
    /// </summary>
    public FlushUserRolesModel[]? UpdatesUsersRoles { get; set; }

    /// <summary>
    /// Разрешён ли пользователю вход.
    /// </summary>
    public bool UserAuthorizationIsAllowed(string userEmail)
    {
        return MailAddress.TryCreate(userEmail, out _) &&
            (DenyAuthorization?.IsDeny != true || UpdatesUsersRoles?.Any(x => x.EmailUser.Equals(userEmail) && x.SetRoles?.Any(y => y.Equals(GlobalStaticConstants.Roles.Admin, StringComparison.OrdinalIgnoreCase) || y.Equals("administrator", StringComparison.OrdinalIgnoreCase)) == true) == true);
    }

    /// <summary>
    /// Разрешён ли пользователю регистрация.
    /// </summary>
    public bool UserRegistrationIsAllowed(string userEmail)
    {
        return MailAddress.TryCreate(userEmail, out _) &&
            (DenyRegistration?.IsDeny != true || UpdatesUsersRoles?.Any(x => x.EmailUser.Equals(userEmail) && x.SetRoles?.Any(y => y.Equals(GlobalStaticConstants.Roles.Admin, StringComparison.OrdinalIgnoreCase) || y.Equals("administrator", StringComparison.OrdinalIgnoreCase)) == true) == true);
    }
}