////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// OrderAttendancesModelDB
/// </summary>
public class RecordsAttendanceModelDB : OrderDocumentBaseModelDB
{
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
    /// DateExecute
    /// </summary>
    [Required]
    public required DateOnly DateExecute { get; set; }

    /// <summary>
    /// StartPart
    /// </summary>
    [Required]
    public required TimeOnly StartPart { get; set; }

    /// <summary>
    /// EndPart
    /// </summary>
    [Required]
    public required TimeOnly EndPart { get; set; }

    /// <summary>
    /// Имя контекста для разделения различных селекторов независимо друг от друга
    /// </summary>
    public string? ContextName { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[{DateExecute}] ({StartPart}-{EndPart}): offer {(Offer is null ? $"#{OfferId}" : Offer.GetName())}";
    }
}