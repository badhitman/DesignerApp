////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// System entry
/// </summary>
[Index(nameof(ProjectId))]
public class EntryConstructedModel : EntryDescriptionModel
{
    /// <summary>
    /// Project
    /// </summary>
    public ProjectModelDb? Project { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public required int ProjectId { get; set; }

    /// <summary>
    /// Is Shared
    /// </summary>
    public bool IsShared { get; set; }

    /// <inheritdoc/>
    public static EntryConstructedModel Build(EntryModel sender, int projectId, string? description = null)
    {
        return new()
        {
            Id = sender.Id,
            Name = sender.Name,
            ProjectId = projectId,
            Description = description
        };
    }
}