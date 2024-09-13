////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
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
    public string EmailUser { get; set; } = default!;

    /// <summary>
    /// Роли, которые нужно назначить пользователю. Если null, то пользователь лишится всех ролей
    /// </summary>
    public string[] SetRoles { get; set; } = default!;
}