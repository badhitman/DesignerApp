////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////


using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// StorageArticleModelDB
/// </summary>
[Index(nameof(CreatedAtUTC)), Index(nameof(UpdatedAtUTC)), Index(nameof(AuthorIdentityId))]
public class StorageArticleModelDB : EntryDescriptionModel
{
    /// <summary>
    /// CreatedAtUTC
    /// </summary>
    public DateTime CreatedAtUTC { get; set; }

    /// <summary>
    /// CreatedAtUTC
    /// </summary>
    public DateTime UpdatedAtUTC { get; set; }

    /// <summary>
    /// AuthorIdentityId
    /// </summary>
    public required string AuthorIdentityId { get; set; }

    /// <summary>
    /// Tags
    /// </summary>
    public List<ArticleTagModelDB>? Tags { get; set; }
}