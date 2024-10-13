////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// FileTagModelDB
/// </summary>
[Index(nameof(NormalizedNameUpper))]
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

    /// <summary>
    /// NormalizedNameUpper
    /// </summary>
    public string NormalizedNameUpper { get; set; } = default!;
}