////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Элемент перечисления
/// </summary>
[Index(nameof(SystemName)), Index(nameof(SortIndex))]
[Index(nameof(Name), nameof(ParentId), IsUnique = true)]
[Index(nameof(SystemName), nameof(ParentId), IsUnique = true)]
public class ConstructorFormDirectoryElementModelDB : EntryDescriptionModel
{
    /// <summary>
    /// System name
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.NAME_SPACE_TEMPLATE, ErrorMessage = GlobalStaticConstants.NAME_SPACE_TEMPLATE_MESSAGE)]
    public required string SystemName { get; set; }

    /// <summary>
    /// Перечисление-владелец
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// Перечисление-владелец
    /// </summary>
    public ConstructorFormDirectoryModelDB? Parent { get; set; }

    /// <summary>
    /// Сортировка
    /// </summary>
    public required int SortIndex { get; set; }
}