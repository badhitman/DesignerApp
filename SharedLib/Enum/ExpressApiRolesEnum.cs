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

    /// <summary>
    /// Изменение данных организаций (сервис: Commerce)
    /// </summary>
    [Description("Изменение данных организаций (сервис: Commerce)")]
    OrganizationsWriteCommerce = 20,

    /// <summary>
    /// Чтение данных платежей (сервис: Commerce)
    /// </summary>
    [Description("Чтение данных платежей (сервис: Commerce)")]
    PaymentsReadCommerce = 30,

    /// <summary>
    /// Изменение данных платежей (сервис: Commerce)
    /// </summary>
    [Description("Изменение данных платежей (сервис: Commerce)")]
    PaymentsWriteCommerce = 40,

    /// <summary>
    /// Чтение данных доставки (сервис: Commerce)
    /// </summary>
    [Description("Чтение данных заказов (сервис: Commerce)")]
    OrdersReadCommerce = 50,

    /// <summary>
    /// Изменение данных платежей (сервис: Commerce)
    /// </summary>
    [Description("Изменение доставки заказов (сервис: Commerce)")]
    OrdersWriteCommerce = 60,
}