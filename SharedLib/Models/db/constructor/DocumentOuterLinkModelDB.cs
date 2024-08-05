////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Document outer project link
/// </summary>
public class DocumentOuterLinkModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <inheritdoc/>
    public DocumentSchemeConstructorModelDB? Document { get; set; }
    /// <inheritdoc/>
    public int DocumentId { get; set; }

    /// <inheritdoc/>
    public ProjectConstructorModelDB? Project { get; set; }

    /// <inheritdoc/>
    public int ProjectId { get; set; }
}