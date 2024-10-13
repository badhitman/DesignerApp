////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// ArticleTagModelDB
/// </summary>
[Index(nameof(NormalizedNameUpper))]
public class ArticleTagModelDB : EntryModel
{
    /// <summary>
    /// Owner Article
    /// </summary>
    public ArticleModelDB? OwnerArticle { get; set; }
    /// <summary>
    /// Owner Article (FK)
    /// </summary>
    public int OwnerArticleId { get; set; }

    /// <summary>
    /// NormalizedNameUpper
    /// </summary>
    public string NormalizedNameUpper { get; set; } = default!;
}