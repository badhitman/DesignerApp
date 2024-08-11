////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// IssueTelegramModelDB
/// </summary>
[Index(nameof(AuthorIdentityUserId), nameof(ExecutorIdentityUserId), nameof(LastUpdateAt))]
public class IssueHelpdeskModelDB : EntryDescriptionModel
{
    /// <summary>
    /// Шаг/статус обращения: "Создан", "В работе", "На проверке" и "Готово"
    /// </summary>
    public HelpdeskIssueStepsEnum StepIssue { get; set; }

    /// <summary>
    /// IdentityUserId
    /// </summary>
    public required string? AuthorIdentityUserId { get; set; }

    /// <summary>
    /// Исполнитель
    /// </summary>
    public string? ExecutorIdentityUserId { get; set; }

    /// <summary>
    /// Rubric Issue
    /// </summary>
    public required int? RubricIssueId { get; set; }

    /// <summary>
    /// Rubric Issue
    /// </summary>
    public RubricIssueHelpdeskModelDB? RubricIssue { get; set; }

    /// <summary>
    /// CreatedAt
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// LastUpdateAt
    /// </summary>
    public DateTime LastUpdateAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// AnonymAccess
    /// </summary>
    public List<AnonymTelegramAccessHelpdeskModelDB>? AnonymAccess { get; set; }

    /// <summary>
    /// Subscribers
    /// </summary>
    public List<SubscriberIssueHelpdeskModelDB>? Subscribers { get; set; }

    /// <summary>
    /// Messages
    /// </summary>
    public List<IssueMessageHelpdeskModelDB>? Messages { get; set; }
}