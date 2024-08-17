////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// PulseIssueBaseModel
/// </summary>
public class PulseIssueBaseModel
{
    /// <summary>
    /// Issue
    /// </summary>
    public int IssueId { get; set; }

    /// <summary>
    /// Тип события
    /// </summary>
    public PulseIssuesTypesEnum PulseType { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    [Required]
    public required string Description { get; set; }

    /// <summary>
    /// Tag
    /// </summary>
    public string? Tag { get; set; }
}
