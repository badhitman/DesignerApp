////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// StorageFileMiddleModel
/// </summary>
[Index(nameof(PointId)), Index(nameof(AuthorIdentityId)), Index(nameof(FileName))]
public class StorageFileMiddleModel : StorageBaseModelDB
{
    /// <summary>
    /// AuthorIdentityId
    /// </summary>
    public required string AuthorIdentityId { get; set; }

    /// <summary>
    /// PointId
    /// </summary>
    public required string PointId { get; set; }

    /// <summary>
    /// FileName
    /// </summary>
    public required string FileName { get; set; }
}
