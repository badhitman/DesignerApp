namespace SharedLib;

/// <summary>
/// Роли
/// </summary>
public class RoleInfoModel : EntryAltModel
{
    /// <summary>
    /// Title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Количество пользователей в роли
    /// </summary>
    public int UsersCount { get; set; }
}