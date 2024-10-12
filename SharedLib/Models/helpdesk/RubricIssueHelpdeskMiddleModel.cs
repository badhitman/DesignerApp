////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Рубрика (прослойка IssueHelpdesk)
/// </summary>
public class RubricIssueHelpdeskMiddleModel : RubricLayerModel
{
    /// <inheritdoc/>
    public List<RubricIssueHelpdeskModelDB>? NestedRubrics { get; set; }
}