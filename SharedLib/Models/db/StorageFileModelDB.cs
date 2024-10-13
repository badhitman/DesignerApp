////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <inheritdoc/>
[Index(nameof(ReferrerMain))]
[Index(nameof(NormalizedFileNameUpper))]
public class StorageFileModelDB : StorageFileMiddleModel
{
    /// <summary>
    /// ToUpper
    /// </summary>
    public string? NormalizedFileNameUpper { get; set; }

    /// <summary>
    /// FileLength
    /// </summary>
    public long FileLength { get; set; }

    /// <summary>
    /// ContentType
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Referrer (main/init)
    /// </summary>
    public string? ReferrerMain { get; set; }

    /// <summary>
    /// Tags
    /// </summary>
    public List<FileTagModelDB>? Tags { get; set; }
}