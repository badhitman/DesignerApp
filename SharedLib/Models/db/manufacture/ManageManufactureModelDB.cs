using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// ManageManufacture
/// </summary>
public class ManageManufactureModelDB : CodeGeneratorConfigModel, ICloneable
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <inheritdoc/>
    public required string UserId { get; set; }

    /// <summary>
    /// Проект
    /// </summary>
    public ProjectConstructorModelDB? Project { get; set; }
    /// <summary>
    /// FK: Проект
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// System names
    /// </summary>
    public List<ManufactureSystemNameModelDB>? SystemNames { get; set; }

    /// <inheritdoc/>
    public object Clone()
    {
        return new ManageManufactureModelDB()
        {
            Namespace = Namespace,
            Project = Project,
            SystemNames = SystemNames is null ? [] : [.. SystemNames.Select(x => x.Clone()).Cast<ManufactureSystemNameModelDB>()],
            UserId = UserId,
            Id = Id,
            AccessDataDirectoryPath = AccessDataDirectoryPath,
            BlazorDirectoryPath = BlazorDirectoryPath,
            BlazorSplitFiles = BlazorSplitFiles,
            DocumentsMastersDbDirectoryPath = DocumentsMastersDbDirectoryPath,
            EnumDirectoryPath = EnumDirectoryPath,
            ProjectId = ProjectId,
        };
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj is ManageManufactureModelDB other)
            return
                    Id == other.Id &&
                    DocumentsMastersDbDirectoryPath == other.DocumentsMastersDbDirectoryPath &&
                    UserId == other.UserId &&
                    AccessDataDirectoryPath == other.AccessDataDirectoryPath &&
                    Namespace == other.Namespace &&
                    EnumDirectoryPath == other.EnumDirectoryPath &&
                    BlazorDirectoryPath == other.BlazorDirectoryPath &&
                    BlazorSplitFiles == other.BlazorSplitFiles &&
                    ProjectId == other.ProjectId;

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => $"{Id} {BlazorDirectoryPath} {Namespace} {AccessDataDirectoryPath} {DocumentsMastersDbDirectoryPath} {EnumDirectoryPath} {UserId} {ProjectId}".GetHashCode();

    /// <summary>
    /// Reload
    /// </summary>
    public void Reload(ManageManufactureModelDB other)
    {
        Id = other.Id;
        DocumentsMastersDbDirectoryPath = other.DocumentsMastersDbDirectoryPath;
        UserId = other.UserId;
        AccessDataDirectoryPath = other.AccessDataDirectoryPath;
        Namespace = other.Namespace;
        EnumDirectoryPath = other.EnumDirectoryPath;
        BlazorDirectoryPath = other.BlazorDirectoryPath;
        BlazorSplitFiles = other.BlazorSplitFiles;
        ProjectId = other.ProjectId;
    }
}