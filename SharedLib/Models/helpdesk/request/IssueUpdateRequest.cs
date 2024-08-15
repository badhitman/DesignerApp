////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IssueUpdateRequest
/// </summary>
public class IssueUpdateRequest : EntryDescriptionModel
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