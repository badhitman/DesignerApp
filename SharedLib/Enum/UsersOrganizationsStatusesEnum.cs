////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Статус пользователя в компании
/// </summary>
public enum UsersOrganizationsStatusesEnum
{
    /// <summary>
    /// None
    /// </summary>
    [Description("None")]
    None = 0,

    /// <summary>
    /// Обычный сотрудник
    /// </summary>
    [Description("Обычный сотрудник")]
    SimpleUnit = 10,

    /// <summary>
    /// Менеджер
    /// </summary>
    [Description("Менеджер")]
    Manager = 100,
}