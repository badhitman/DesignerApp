////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрос подбора заказов (поиск по параметрам)
/// </summary>
public class OrdersSelectRequestModel : DocumentsSelectRequestBaseModel
{
    /// <summary>
    /// Статусы: "Создан", "В работе", "На проверке" и "Готово"
    /// </summary>
    public StatusesDocumentsEnum[]? StatusesFilter { get; set; }

    /// <summary>
    /// Дата обновления должна быть не меньше указанной
    /// </summary>
    public DateTime? AfterDateUpdate { get; set; }

    /// <summary>
    /// Фильтр по адресам организаций (вкладки в заказах)
    /// </summary>
    public int? AddressForOrganizationFilter { get; set; }

    /// <summary>
    /// Фильтр по организации
    /// </summary>
    public int? OrganizationFilter { get; set; }
}