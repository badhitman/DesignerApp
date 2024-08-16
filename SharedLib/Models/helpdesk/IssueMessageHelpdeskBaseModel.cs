////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public int IssueId { get; set; }
}