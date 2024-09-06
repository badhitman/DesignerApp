////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// OrdersSelectRequestModel
/// </summary>
public class OrdersSelectRequestModel
{
    /// <summary>
    /// Дата обновления должна быть не меньше указанной
    /// </summary>
    public DateTime? AfterDateUpdate { get; set; }

    /// <summary>
    /// AddressForOrganization
    /// </summary>
    public int? AddressForOrganizationFilter { get; set; }

    /// <summary>
    /// OrganizationFilter
    /// </summary>
    public int? OrganizationFilter { get; set; }

    /// <summary>
    /// GoodsFilter
    /// </summary>
    public int? GoodsFilter { get; set; }

    /// <summary>
    /// OfferFilter
    /// </summary>
    public int? OfferFilter { get; set; }

    /// <summary>
    /// Получить корзины клиента
    /// </summary>
    /// <remarks>
    /// Заказы, у которых не указан связанная заявка в HelpDesk считается не оформленным заказом, а предварительным (корзина)
    /// </remarks>
    public bool? IsCartFilter { get; set; }

    /// <summary>
    /// IncludeExternalData
    /// </summary>
    public bool IncludeExternalData {  get; set; }
}