////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// IssueTelegramModelDB
/// </summary>
[Index(nameof(AuthorTelegramUserId), nameof(AuthorIdentityUserId), nameof(ExecutorIdentityUserId), nameof(LastUpdateAt))]
public class IssueModelDB : EntryDescriptionModel
{
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
    public IssueThemeModelDB? IssueTheme { get; set; }

    /// <summary>
    /// LastUpdateAt
    /// </summary>
    public required DateTime LastUpdateAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Subscribers
    /// </summary>
    public List<SubscriberIssueModelDB>? Subscribers { get; set; }

    /// <summary>
    /// Messages
    /// </summary>
    public List<IssueMessageModelDB>? Messages { get; set; }
}