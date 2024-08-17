////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Событие
/// </summary>
[Index(nameof(AuthorUserIdentityId))]
[Index(nameof(CreatedAt))]
[Index(nameof(PulseType))]
public class PulseIssueModelDB : PulseIssueBaseModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public IssueHelpdeskModelDB? Issue { get; set; }

    /// <summary>
    /// User Id (Identity)
    /// </summary>
    public required string AuthorUserIdentityId { get; set; }

    /// <summary>
    /// CreatedAt
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}