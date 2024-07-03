using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// ManageManufacture
/// </summary>
public class ManageManufactureModelDB : CodeGeneratorConfigModel
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


    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj is ManageManufactureModelDB other)
            return
                    Id == other.Id &&
                    ControllersDirectoryPath == other.ControllersDirectoryPath &&
                    DocumentsMastersDbDirectoryPath == other.DocumentsMastersDbDirectoryPath &&
                    UserId == other.UserId &&
                    AccessDataDirectoryPath == other.AccessDataDirectoryPath &&
                    Namespace == other.Namespace &&
                    EnumDirectoryPath == other.EnumDirectoryPath &&
                    ProjectId == other.ProjectId;

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => $"{Id} {Namespace} {AccessDataDirectoryPath} {ControllersDirectoryPath} {DocumentsMastersDbDirectoryPath} {EnumDirectoryPath} {UserId} {ProjectId}".GetHashCode();

    /// <summary>
    /// Reload
    /// </summary>
    public void Reload(ManageManufactureModelDB other)
    {
        Id = other.Id;
        ControllersDirectoryPath = other.ControllersDirectoryPath;
        DocumentsMastersDbDirectoryPath = other.DocumentsMastersDbDirectoryPath;
        UserId = other.UserId;
        AccessDataDirectoryPath = other.AccessDataDirectoryPath;
        Namespace = other.Namespace;
        EnumDirectoryPath = other.EnumDirectoryPath;
        ProjectId = other.ProjectId;
    }
}