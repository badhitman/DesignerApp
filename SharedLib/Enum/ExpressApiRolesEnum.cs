////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Роли для экспресс авторизации (кастомная авторизация по токену)
/// </summary>
public enum ExpressApiRolesEnum
{
    /// <summary>
    /// Чтение данных организаций (сервис: Commerce)
    /// </summary>
    [Description("Чтение данных организаций (сервис: Commerce)")]
    OrganizationsReadCommerce = 10,
}