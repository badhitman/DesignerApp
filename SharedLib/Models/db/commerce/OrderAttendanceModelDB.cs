////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// OrderAttendancesModelDB
/// </summary>
public class OrderAttendanceModelDB : OrderDocumentBaseModelDB
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
    /// DateAttendance
    /// </summary>
    public required DateOnly DateAttendance { get; set; }

    /// <summary>
    /// StartPart
    /// </summary>
    [Required]
    public required DateTime StartPart { get; set; }

    /// <summary>
    /// EndPart
    /// </summary>
    [Required]
    public required DateTime EndPart { get; set; }
}
