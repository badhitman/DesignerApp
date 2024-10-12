////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// RubricArticleJoinModelDB
/// </summary>
public class RubricArticleJoinModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Rubric
    /// </summary>
    public RubricArticleModelDB? Rubric { get; set; }

    /// <summary>
    /// Rubric [FK]
    /// </summary>
    public int RubricId { get; set; }

    /// <summary>
    /// Article
    /// </summary>
    public StorageArticleModelDB? Article { get; set; }
    /// <summary>
    /// Article [FK]
    /// </summary>
    public int ArticleId { get; set; }
}