////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// IssueMessageHelpdeskBaseModel
/// </summary>
public class IssueMessageHelpdeskBaseModel
{
    /// <summary>
    /// IdentityUserId
    /// </summary>
    [Required] 
    public required string IdentityUserId { get; set; }

    /// <summary>
    /// MessageText
    /// </summary>
    [Required]
    public required string MessageText { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public int IssueId { get; set; }
}