////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Form of document snapshot
/// </summary>
public class FormSnapshotModelDB : SortableSystemSnapshotModelDB
{
    /// <summary>
    ///Tab [PARENT OWNER]
    /// </summary>
    public TabSnapshotModelDB? Owner { get; set; }

    /// <inheritdoc/>
    public List<BaseFieldModel>? Fields { get; set; }
}