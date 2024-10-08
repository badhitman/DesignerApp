﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Событие изменения в обращении
/// </summary>
[Index(nameof(PulseType))]
[Index(nameof(Tag))]
public class PulseIssueLowModel
{
    /// <summary>
    /// Тип события
    /// </summary>
    public PulseIssuesTypesEnum PulseType { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    [Required]
    public required string Description { get; set; }

    /// <summary>
    /// Tag
    /// </summary>
    public string? Tag { get; set; }
}