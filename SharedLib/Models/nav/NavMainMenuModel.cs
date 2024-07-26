////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Меню (основное)
/// </summary>
public class NavMainMenuModel
{
    /// <summary>
    /// Элементы меню
    /// </summary>
    public required NavItemModel[] NavMenuItems { get; set; }

    /// <summary>
    /// Элементы меню (нижнего/второстепенного/дополнительного)
    /// </summary>
    /// <remarks>
    /// Для отладки. Подключение второго меню, которое было создано генератором
    /// </remarks>
    public NavItemModel[]? BottomNavMenuItems { get; set; }
}