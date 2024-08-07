////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// IssueTelegramModelDB
/// </summary>
[Index(nameof(AuthorTelegramUserId), nameof(AuthorIdentityUserId), nameof(ExecutorIdentityUserId), nameof(LastUpdateAt))]
public class IssueHelpdeskModelDB : EntryDescriptionModel
{
    /// <summary>
    /// Шаг/статус обращения: "Создан", "В работе", "На проверке" и "Готово"
    /// </summary>
    public HelpdeskIssueStepsEnum StepIssue { get; set; }

    /// <summary>
    /// TelegramUserId
    /// </summary>
    public required long? AuthorTelegramUserId { get; set; }

    /// <summary>
    /// IdentityUserId
    /// </summary>
    public required string? AuthorIdentityUserId { get; set; }

    /// <summary>
    /// Исполнитель
    /// </summary>
    public string? ExecutorIdentityUserId { get; set; }

    /// <summary>
    /// IssueTheme
    /// </summary>
    public required int IssueThemeId { get; set; }

    /// <summary>
    /// IssueTheme
    /// </summary>
    public IssueThemeHelpdeskModelDB? IssueTheme { get; set; }

    /// <summary>
    /// CreatedAt
    /// </summary>
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// LastUpdateAt
    /// </summary>
    public required DateTime LastUpdateAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// AnonymAccess
    /// </summary>
    public AnonymTelegramAccessHelpdeskModelDB? AnonymAccess { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? TokenAccessId { get; set; }

    /// <summary>
    /// Subscribers
    /// </summary>
    public List<SubscriberIssueHelpdeskModelDB>? Subscribers { get; set; }

    /// <summary>
    /// Messages
    /// </summary>
    public List<IssueMessageHelpdeskModelDB>? Messages { get; set; }
}