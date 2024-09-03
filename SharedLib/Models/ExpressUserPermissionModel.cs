////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Права доступа экспресс авторизации
/// </summary>
public class ExpressUserPermissionModel
{
    /// <summary>
    /// имя/email пользователя (для логов)
    /// </summary>
    public string? User { get; set; }

    /// <summary>
    /// Токен/пароль доступа
    /// </summary>
    public string? Secret { get; set; }

    /// <summary>
    /// Роль доступные пользователю
    /// </summary>
    public IEnumerable<ExpressApiRolesEnum>? Roles { get; set; }
}