////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
///  Sortable
/// </summary>
public class SortableSystemSnapshotModelDB : SystemEntryDescriptionOwnedModel
{
    /// <inheritdoc/>
    public required int SortIndex { get; set; }
}
