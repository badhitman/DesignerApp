﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// WorkScheduleBaseModelDB
/// </summary>
[Index(nameof(StartPart), nameof(EndPart))]
public class WorkScheduleBaseModelDB : UniversalLayerModel
{
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

    /// <summary>
    /// Ёмкость очереди (0 - безлимитное)
    /// </summary>
    public uint QueueCapacity { get; set; }
}