////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Tab of document - snapshot
/// </summary>
public class TabSnapshotModelDB : SystemEntryDescriptionOwnedModel
{
    /// <inheritdoc/>
    public DocumentSnapshotModelDB? Owner { get; set; }

    /// <inheritdoc/>
    public FormSnapshotModelDB[]? Forms { get; set; }
}