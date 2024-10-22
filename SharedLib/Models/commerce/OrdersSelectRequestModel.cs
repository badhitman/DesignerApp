////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрос подбора заказов (поиск по параметрам)
/// </summary>
public class OrdersSelectRequestModel
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

    /// <summary>
    /// Фильтр по номенклатуре
    /// </summary>
    public int? GoodsFilter { get; set; }

    /// <summary>
    /// Фильтр по коммерческому предложению
    /// </summary>
    public int? OfferFilter { get; set; }

    /// <summary>
    /// Загрузить дополнительные данные для заказов
    /// </summary>
    /// <remarks>
    /// .Include(x => x.Organization)
    /// .Include(x => x.AddressesTabs)
    /// .ThenInclude(x => x.Rows)
    /// .ThenInclude(x => x.Offer)
    /// .ThenInclude(x => x.Goods)
    /// </remarks>
    public bool IncludeExternalData { get; set; }

    /// <summary>
    /// Заказы для заявки из СДЭК
    /// </summary>
    public int[]? IssueIds { get; set; }
}