////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// FileTagModelDB
/// </summary>
public class FileTagModelDB : EntryModel
{
    /// <summary>
    /// OwnerFile
    /// </summary>
    public StorageFileModelDB? OwnerFile { get; set; }
    /// <summary>
    /// OwnerFile (FK)
    /// </summary>
    public int OwnerFileId { get; set; }
}