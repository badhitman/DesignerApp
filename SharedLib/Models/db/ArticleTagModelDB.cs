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
    public ArticleModelDB? OwnerArticle { get; set; }
    /// <summary>
    /// Owner Article (FK)
    /// </summary>
    public int OwnerArticleId { get; set; }
}