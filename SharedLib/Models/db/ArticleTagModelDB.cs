////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ArticleTagModelDB
/// </summary>
public class ArticleTagModelDB : EntryModel
{
    /// <summary>
    /// Owner Article
    /// </summary>
    public StorageArticleModelDB? OwnerArticle { get; set; }
    /// <summary>
    /// Owner Article (FK)
    /// </summary>
    public int OwnerArticleId { get; set; }
}