////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// RowOfBaseDocumentModelDB
/// </summary>
[Index(nameof(Quantity))]
public abstract class RowOfBaseDocumentModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Торговое предложение
    /// </summary>
    public OfferModelDB? Offer { get; set; }

    /// <summary>
    /// Offer
    /// </summary>
    public int OfferId { get; set; }

    /// <summary>
    /// Номенклатура
    /// </summary>
    public NomenclatureModelDB? Nomenclature { get; set; }

    /// <summary>
    /// Nomenclature
    /// </summary>
    public int NomenclatureId { get; set; }

    /// <summary>
    /// Количество
    /// </summary>
    public decimal Quantity { get; set; }
}