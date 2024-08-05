////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Document snapshot
/// </summary>
public class DocumentSnapshotModelDB : SystemEntryDescriptionOwnedModel
{
    /// <summary>
    /// Snapshot of project [PARENT OWNER]
    /// </summary>
    public ProjectSnapshotModelDB? Owner { get; set; }

    /// <summary>
    /// Tabs
    /// </summary>
    public required IEnumerable<TabSnapshotModelDB> Tabs { get; set; }
}