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
    public RubricIssueHelpdeskModelDB? Rubric { get; set; }

    /// <summary>
    /// Rubric [FK]
    /// </summary>
    public int RubricId { get; set; }

    /// <summary>
    /// Article
    /// </summary>
    public ArticleModelDB? Article { get; set; }
    /// <summary>
    /// Article [FK]
    /// </summary>
    public int ArticleId { get; set; }
}