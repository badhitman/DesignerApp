using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Справочник/список
/// </summary>
[Index(nameof(Name), IsUnique = true)]
public class ConstructorFormDirectoryModelDB : SystemEntryModel
{
    /// <inheritdoc/>
    public static ConstructorFormDirectoryModelDB Build(SystemEntryModel entry)
        => new()
        {
            Name = entry.Name,
            SystemName = entry.SystemName,
            Description = entry.Description,
            Id = entry.Id,
            IsDeleted = entry.IsDeleted,
            Project = entry.Project,
            ProjectId = entry.ProjectId,
        };

    /// <summary>
    /// Элементы справочника/списка
    /// </summary>
    public List<ConstructorFormDirectoryElementModelDB>? Elements { get; set; }

    /// <summary>
    /// Связи форм со списками/связями
    /// </summary>
    public List<ConstructorFormDirectoryLinkModelDB>? FormsDirectoriesLinks { get; set; }
}
