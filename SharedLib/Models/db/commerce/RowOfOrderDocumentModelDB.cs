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
    /// AddressForOrderTab
    /// </summary>
    public AddressForOrderModelDb? AddressForOrderTab { get; set; }
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
    /// Offer
    /// </summary>
    public OfferGoodModelDB? Offer { get; set; }

    /// <summary>
    /// Offer
    /// </summary>
    public int OfferId { get; set; }

    /// <summary>
    /// Goods
    /// </summary>
    public GoodsModelDB? Goods { get; set; }

    /// <summary>
    /// Goods
    /// </summary>
    public int GoodsId { get; set; }

    /// <summary>
    /// Quantity
    /// </summary>
    public uint Quantity { get; set; }
}