////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Рубрика (прослойка Article)
/// </summary>
public class RubricArticleMiddleModel : RubricLayerModel
{
    /// <inheritdoc/>
    public List<RubricArticleModelDB>? NestedRubrics { get; set; }
}