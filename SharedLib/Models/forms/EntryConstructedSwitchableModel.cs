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
[Index(nameof(SystemName))]
public class EntryConstructedSwitchableModel : EntryDescriptionSwitchableModel
{
    /// <summary>
    /// System name
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.NAME_SPACE_TEMPLATE, ErrorMessage = GlobalStaticConstants.NAME_SPACE_TEMPLATE_MESSAGE)]
    public required string SystemName { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public ProjectConstructorModelDb? Project { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public required int ProjectId { get; set; }

    /// <inheritdoc/>
    public static EntryConstructedSwitchableModel Build(SystemEntrySwitchableModel sender, int projectId)
    {
        return new()
        {
            Id = sender.Id,
            Name = sender.Name,
            SystemName = sender.SystemName,
            ProjectId = projectId,
            IsDisabled = sender.IsDisabled,
        };
    }
}