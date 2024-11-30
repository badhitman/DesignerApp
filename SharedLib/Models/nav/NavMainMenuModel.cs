////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Меню (основное)
/// </summary>
public class NavMainMenuModel
{
    /// <summary>
    /// Элементы меню (верхнее)
    /// </summary>
    public NavItemModel[] TopNavMenuItems { get; set; } = default!;

    /// <summary>
    /// Элементы меню (слева)
    /// </summary>
    public NavItemModel[] NavMenuItems { get; set; } = default!;

    /// <summary>
    /// Элементы меню (нижнего/второстепенного/дополнительного)
    /// </summary>
    /// <remarks>
    /// Для отладки. Подключение второго меню, которое было создано генератором
    /// </remarks>
    public NavItemModel[]? BottomNavMenuItems { get; set; }
}