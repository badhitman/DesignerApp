////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// System entry
/// </summary>
public class EntryConstructedModel : EntryDescriptionModel
{
    /// <summary>
    /// Project
    /// </summary>
    public ProjectConstructorModelDB? Project { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public required int ProjectId { get; set; }

    /// <inheritdoc/>
    public static EntryConstructedModel Build(EntryModel sender, int projectId)
    {
        return new()
        {
            Id = sender.Id,
            Name = sender.Name,
            ProjectId = projectId,
        };
    }
}