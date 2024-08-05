////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Snapshot of project
/// </summary>
public class ProjectSnapshotModelDB : EntryConstructedModel
{
    /// <inheritdoc/>
    public required string UserId { get; set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Token
    /// </summary>
    public Guid Token { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Directories
    /// </summary>
    public required ICollection<DirectoryEnumSnapshotModelDB> Directories { get; set; }

    /// <summary>
    /// Documents
    /// </summary>
    public required ICollection<DocumentSnapshotModelDB> Documents { get; set; }
}