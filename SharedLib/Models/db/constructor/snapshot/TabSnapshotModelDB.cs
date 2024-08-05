////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Tab of document - snapshot
/// </summary>
public class TabSnapshotModelDB : SortableSystemSnapshotModelDB
{
    /// <inheritdoc/>
    public DocumentSnapshotModelDB? Owner { get; set; }

    /// <inheritdoc/>
    public IEnumerable<FormSnapshotModelDB>? Forms { get; set; }
}