////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IssueMessageHelpdeskBaseModel
/// </summary>
public class IssueMessageHelpdeskBaseModel : UserCrossIdsModel
{
    /// <summary>
    /// MessageText
    /// </summary>
    public required string MessageText { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public int IssueId { get; set; }
}
