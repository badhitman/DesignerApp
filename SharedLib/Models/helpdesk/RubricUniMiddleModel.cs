////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// RubricUniMiddleModel
/// </summary>
public class RubricUniMiddleModel : UniversalLayerModel
{
    /// <inheritdoc/>
    public List<RubricIssueHelpdeskModelDB>? NestedRubrics { get; set; }
}