////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Рубрика (прослойка IssueHelpdesk)
/// </summary>
public class RubricIssueHelpdeskMiddleModel : UniversalLayerModel
{
    /// <inheritdoc/>
    public List<RubricIssueHelpdeskModelDB>? NestedRubrics { get; set; }
}