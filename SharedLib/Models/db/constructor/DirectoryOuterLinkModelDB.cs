////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Form outer project link
/// </summary>
public class DirectoryOuterLinkModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <inheritdoc/>
    public DirectoryConstructorModelDB? Directory { get; set; }
    /// <inheritdoc/>
    public int FormId { get; set; }

    /// <inheritdoc/>
    public ProjectModelDb? Project { get; set; }

    /// <inheritdoc/>
    public int ProjectId { get; set; }
}