////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// IssueMessageModelDB
/// </summary>
[Index(nameof(IdentityUserId))]
[Index(nameof(CreatedAt), nameof(LastUpdateAt))]
public class IssueMessageHelpdeskModelDB : IssueMessageHelpdeskBaseModel
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
    /// CreatedAt
    /// </summary>
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// LastUpdateAt
    /// </summary>
    public required DateTime LastUpdateAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Отметки как ответ
    /// </summary>
    public List<VoteHelpdeskModelDB>? Vote { get; set; }
}