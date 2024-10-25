////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// IssueHelpdeskModel
/// </summary>
[Index(nameof(LastUpdateAt)), Index(nameof(CreatedAt)), Index(nameof(StepIssue)), Index(nameof(NormalizedDescriptionUpper)), Index(nameof(AuthorIdentityUserId))]
public class IssueHelpdeskModel : EntryDescriptionModel
{
    /// <summary>
    /// Шаг/статус обращения: "Создан", "В работе", "На проверке" и "Готово"
    /// </summary>
    public StatusesDocumentsEnum StepIssue { get; set; }

    /// <summary>
    /// IdentityUserId
    /// </summary>
    public required string AuthorIdentityUserId { get; set; }

    /// <inheritdoc/>
    public string? NormalizedDescriptionUpper { get; set; }

    /// <summary>
    /// Исполнитель
    /// </summary>
    public string? ExecutorIdentityUserId { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Subscribers
    /// </summary>
    public List<SubscriberIssueHelpdeskModelDB>? Subscribers { get; set; }

    /// <summary>
    /// Rubric Issue
    /// </summary>
    public RubricIssueHelpdeskModelDB? RubricIssue { get; set; }

    /// <summary>
    /// CreatedAt (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// LastUpdateAt
    /// </summary>
    public DateTime LastUpdateAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Build
    /// </summary>
    public static IssueHelpdeskModel Build(IssueHelpdeskModelDB sender)
    {
        return new()
        {
            AuthorIdentityUserId = sender.AuthorIdentityUserId,
            ExecutorIdentityUserId = sender.ExecutorIdentityUserId,
            StepIssue = sender.StepIssue,
            Name = sender.Name,
            CreatedAt = sender.CreatedAt,
            LastUpdateAt = sender.LastUpdateAt,
            Description = sender.Description,
            Id = sender.Id,
            ProjectId = sender.ProjectId,
            RubricIssue = sender.RubricIssue,
            Subscribers = sender.Subscribers,
        };
    }
}