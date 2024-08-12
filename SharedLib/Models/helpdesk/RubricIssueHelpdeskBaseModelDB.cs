////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Рубрики для обращений
/// </summary>
public class RubricIssueHelpdeskBaseModelDB : RubricIssueHelpdeskLowModel
{
    /// <summary>
    /// ToUpper
    /// </summary>
    public string? NormalizedNameToUpper { get; set; }

    /// <inheritdoc/>
    public List<RubricIssueHelpdeskModelDB>? NestedRubrics { get; set; }
}