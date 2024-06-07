////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// System entry
/// </summary>
[Index(nameof(SystemName), nameof(ProjectId), IsUnique = true)]
public class SystemEntryModel : EntryDescriptionModel
{
    /// <summary>
    /// System name
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.NAME_SPACE_TEMPLATE, ErrorMessage = "Системное имя не корректное")]
    public required string SystemName { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public ProjectConstructorModelDb? Project { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public int ProjectId { get; set; }
}