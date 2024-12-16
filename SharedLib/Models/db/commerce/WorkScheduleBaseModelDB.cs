////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// WorkScheduleBaseModelDB
/// </summary>
[Index(nameof(ExecutorIdentityUserId))]
[Index(nameof(StartPart), nameof(EndPart))]
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
    [Required]
    public required TimeSpan StartPart { get; set; }

    /// <summary>
    /// EndPart
    /// </summary>
    [Required]
    public required TimeSpan EndPart { get; set; }
}