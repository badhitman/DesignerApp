﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// System entry
/// </summary>
[Index(nameof(SystemName), nameof(ProjectId), IsUnique = true)]
public class EntryConstructedModel : EntryDescriptionModel
{
    /// <summary>
    /// System name
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.NAME_SPACE_TEMPLATE, ErrorMessage = "Системное имя не корректное. Оно может содержать латинские буквы и цифры. Первым символом должна идти буква")]
    public required string SystemName { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public ProjectConstructorModelDb? Project { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public required int ProjectId { get; set; }
}