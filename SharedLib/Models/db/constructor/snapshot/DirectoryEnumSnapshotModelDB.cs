////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
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
    public required DirectoryEnumElementSnapshotModelDB[] Elements { get; set; }

    /// <summary>
    /// Fields
    /// </summary>
    public FieldAkaDirectorySnapshotModelDB[]? Fields { get; set; }
}