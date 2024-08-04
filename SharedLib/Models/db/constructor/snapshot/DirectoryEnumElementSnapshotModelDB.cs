////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Directory enum snapshot
/// </summary>
public class DirectoryEnumElementSnapshotModelDB : SystemEntryDescriptionOwnedModel
{
    /// <summary>
    /// Owner
    /// </summary>
    public DirectoryEnumSnapshotModelDB? Owner { get; set; }
    
    /// <summary>
    /// Сортировка
    /// </summary>
    public required int SortIndex { get; set; }
}