////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Directory enum snapshot
/// </summary>
public class DirectoryEnumSnapshotModelDB : SystemEntryDescriptionOwnedModel
{
    /// <summary>
    /// Snapshot of project [PARENT OWNER]
    /// </summary>
    public ProjectSnapshotModelDB? Owner { get; set; }

    /// <summary>
    /// Elements
    /// </summary>
    public required List<DirectoryEnumElementSnapshotModelDB> Elements { get; set; }

    /// <summary>
    /// Fields
    /// </summary>
    public List<FieldAkaDirectorySnapshotModelDB>? Fields { get; set; }
}