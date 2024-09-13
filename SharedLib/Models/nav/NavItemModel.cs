////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
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
    public string HrefNav { get; set; } = default!;

    /// <summary>
    /// AuthorizeView -> Roles
    /// </summary>
    public string? AuthorizeViewRoles { get; set; }

    /// <summary>
    /// Текст пункта меню
    /// </summary>
    public string Title { get; set; } = default!;
}