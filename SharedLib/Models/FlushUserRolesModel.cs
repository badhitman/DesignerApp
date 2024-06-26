////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Установка пользователю ролей
/// </summary>
public class FlushUserRolesModel
{
    /// <summary>
    /// Email пользователя
    /// </summary>
    public required string EmailUser { get; set; }

    /// <summary>
    /// Роли, которые нужно назначить пользователю. Если null, то пользователь лишится всех ролей
    /// </summary>
    public required string[] SetRoles { get; set; }
}