////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// RowOfBaseDocumentModelDB
/// </summary>
public class RowOfBaseDocumentModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

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