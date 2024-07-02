using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// ManageManufacture
/// </summary>
public class ManageManufactureModelDB
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
}