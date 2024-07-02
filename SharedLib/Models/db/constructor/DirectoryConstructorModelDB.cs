////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Справочник/список
/// </summary>
[Index(nameof(Name), nameof(ProjectId), IsUnique = true)]
public class DirectoryConstructorModelDB : EntryConstructedModel
{
    /// <inheritdoc/>
    public static DirectoryConstructorModelDB Build(EntryConstructedModel entry)
        => new()
        {
            Name = entry.Name,
            Description = entry.Description,
            Id = entry.Id,
            Project = entry.Project,
            ProjectId = entry.ProjectId,
        };

    /// <summary>
    /// Элементы справочника/списка
    /// </summary>
    public List<ElementOfDirectoryConstructorModelDB>? Elements { get; set; }

    /// <summary>
    /// Связи форм со списками/связями
    /// </summary>
    public List<LinkDirectoryToFormConstructorModelDB>? FormsDirectoriesLinks { get; set; }
}
