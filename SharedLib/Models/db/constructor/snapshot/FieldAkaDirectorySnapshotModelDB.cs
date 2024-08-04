////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Directory field of document snapshot
/// </summary>
public class FieldAkaDirectorySnapshotModelDB : BaseFieldModel
{
    /// <inheritdoc/>
    public DirectoryEnumSnapshotModelDB? Directory { get; set; }

    /// <inheritdoc/>
    public int DirectoryId { get; set; }
}