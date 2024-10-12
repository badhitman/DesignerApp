////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Строка заказа (документа)
/// </summary>
public class RowOfOrderDocumentModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Адрес доставки (из документа заказа)
    /// </summary>
    public TabAddressForOrderModelDb? AddressForOrderTab { get; set; }
    /// <summary>
    /// AddressForOrderTab
    /// </summary>
    public int AddressForOrderTabId { get; set; }

    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public OrderDocumentModelDB? OrderDocument { get; set; }

    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public int? OrderDocumentId { get; set; }

    /// <summary>
    /// Торговое предложение
    /// </summary>
    public OfferGoodModelDB? Offer { get; set; }

    /// <summary>
    /// Offer
    /// </summary>
    public int OfferId { get; set; }

    /// <summary>
    /// Номенклатура
    /// </summary>
    public GoodsModelDB? Goods { get; set; }

    /// <summary>
    /// Goods
    /// </summary>
    public int GoodsId { get; set; }

    /// <summary>
    /// Количество
    /// </summary>
    public int Quantity { get; set; }
}