////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Article Rubrics Set
/// </summary>
public class ArticleRubricsSetModel
{
    /// <summary>
    /// ArticleId
    /// </summary>
    public required int ArticleId { get; set; }

    /// <summary>
    /// RubricsIds
    /// </summary>
    public required int[] RubricsIds { get; set; }
}