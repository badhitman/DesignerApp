////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// WorkScheduleBaseModelDB
/// </summary>
[Index(nameof(ExecutorIdentityUserId))]
public class WorkScheduleBaseModelDB : UniversalLayerModel
{
    /// <summary>
    /// Исполнитель
    /// </summary>
    public string? ExecutorIdentityUserId { get; set; }

    /// <summary>
    /// Nomenclature
    /// </summary>
    public NomenclatureModelDB? Nomenclature { get; set; }
    /// <summary>
    /// Nomenclature
    /// </summary>
    public int? NomenclatureId { get; set; }

    /// <summary>
    /// Offer
    /// </summary>
    public OfferModelDB? Offer { get; set; }
    /// <summary>
    /// Offer
    /// </summary>
    public int? OfferId { get; set; }

    /// <summary>
    /// StartPart
    /// </summary>
    public required TimeOnly StartPart { get; set; }

    /// <summary>
    /// EndPart
    /// </summary>
    public required TimeOnly EndPart { get; set; }
}