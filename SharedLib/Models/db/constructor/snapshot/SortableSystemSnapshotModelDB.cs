////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
///  Sortable
/// </summary>
public class SortableSystemSnapshotModelDB : SystemEntryDescriptionOwnedModel
{
    /// <inheritdoc/>
    public required uint SortIndex { get; set; }
}
