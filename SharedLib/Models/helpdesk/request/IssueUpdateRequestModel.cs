////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Issue Update Request
/// </summary>
public class IssueUpdateRequestModel : EntryDescriptionModel
{
    /// <summary>
    /// Rubric Issue
    /// </summary>
    public required int? RubricIssueId { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    public int ProjectId { get; set; }
}