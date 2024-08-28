////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// StatusChangeRequestModel
/// </summary>
public class StatusChangeRequestModel
{
    /// <summary>
    /// Step
    /// </summary>
    public required HelpdeskIssueStepsEnum Step { get; set; }

    /// <summary>
    /// IssueId
    /// </summary>
    public required int IssueId { get; set; }
}