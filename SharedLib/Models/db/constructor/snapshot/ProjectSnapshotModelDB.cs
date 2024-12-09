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
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    public Guid Token { get; set; }

    /// <summary>
    /// Directories
    /// </summary>
    public required List<DirectoryEnumSnapshotModelDB> Directories { get; set; }

    /// <summary>
    /// Documents
    /// </summary>
    public required List<DocumentSnapshotModelDB> Documents { get; set; }
}