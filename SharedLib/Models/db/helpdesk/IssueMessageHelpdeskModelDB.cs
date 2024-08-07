////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// IssueMessageModelDB
/// </summary>
[Index(nameof(CreatedAt))]
[Index(nameof(LastUpdateAt))]
[Index(nameof(UserIdentityId))]
[Index(nameof(CreatedAt), nameof(LastUpdateAt), nameof(UserIdentityId))]
public class IssueMessageHelpdeskModelDB
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
    /// Issue
    /// </summary>
    public int IssueId { get; set; }

    /// <summary>
    /// MessageText
    /// </summary>
    public required string MessageText { get; set; }

    /// <summary>
    /// CreatedAt
    /// </summary>
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// LastUpdateAt
    /// </summary>
    public required DateTime LastUpdateAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User Id (Identity)
    /// </summary>
    public string? UserIdentityId { get; set; }

    /// <summary>
    /// User Id (Identity)
    /// </summary>
    public long? UserTelegramId { get; set; }

    /// <summary>
    /// Отметки как ответ
    /// </summary>
    public List<MarkAsResponseHelpdeskModelDB>? MarksAsResponse { get; set; }
}