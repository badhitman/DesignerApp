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
    /// MessageText
    /// </summary>
    [Required]
    public required string MessageText { get; set; }

    /// <summary>
    /// Message Id
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public required int IssueId { get; set; }
}