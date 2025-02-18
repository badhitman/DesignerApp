////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Папка синхронизации
/// </summary>
public class SyncDirectoryModelDB : EntryModel
{
    /// <summary>
    /// LocalDirectory
    /// </summary>
    public string? LocalDirectory { get; set; }

    /// <summary>
    /// RemoteDirectory
    /// </summary>
    public string? RemoteDirectory { get; set; }

    /// <summary>
    /// Родитель
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// Родитель
    /// </summary>
    public ApiRestConfigModelDB? Parent { get; set; }
}