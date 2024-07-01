////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Элемент меню навигации
/// </summary>
public class NavItemModel
{
    /// <summary>
    ///  Применение NavLink режим [Match="NavLinkMatch.All"]
    /// </summary>
    public bool IsNavLinkMatchAll { get; set; }

    /// <summary>
    /// Ссылка/href для NavLink
    /// </summary>
    public required string HrefNav {  get; set; }

    /// <summary>
    /// AuthorizeView -> Roles
    /// </summary>
    public string? AuthorizeViewRoles { get; set; }

    /// <summary>
    /// Текст пункта меню
    /// </summary>
    public required string Title { get; set; }
}