////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Строка заказа (документа)
/// </summary>
public class RowOfOrderDocumentModelDB
{
    /// <summary>
    /// AddressOrganization
    /// </summary>
    public AddressOrganizationModelDB? AddressOrganization { get; set; }

    /// <summary>
    /// AddressOrganization
    /// </summary>
    public int AddressOrganizationId { get; set; }

    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public OrderDocumentModelDB? OrderDocument { get; set; }

    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public int? OrderDocumentId { get; set; }

    /// <summary>
    /// Good
    /// </summary>
    public OfferGoodModelDB? Good { get; set; }

    /// <summary>
    /// Good
    /// </summary>
    public int GoodId { get; set; }

    /// <summary>
    /// Quantity
    /// </summary>
    public uint Quantity { get; set; }
}
