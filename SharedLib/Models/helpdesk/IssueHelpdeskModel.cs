////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// IssueHelpdeskModel
/// </summary>
[Index(nameof(LastUpdateAt)), Index(nameof(CreatedAtUTC)), Index(nameof(StatusDocument)), Index(nameof(NormalizedDescriptionUpper)), Index(nameof(AuthorIdentityUserId))]
public class IssueHelpdeskModel : EntryDescriptionModel
{
    /// <summary>
    /// Шаг/статус обращения: "Создан", "В работе", "На проверке" и "Готово"
    /// </summary>
    public StatusesDocumentsEnum StatusDocument { get; set; }

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
    public DateTime CreatedAtUTC { get; set; }

    /// <summary>
    /// LastUpdateAt
    /// </summary>
    public DateTime LastUpdateAt { get; set; }

    /// <summary>
    /// Build
    /// </summary>
    public static IssueHelpdeskModel Build(IssueHelpdeskModelDB sender)
    {
        return new()
        {
            AuthorIdentityUserId = sender.AuthorIdentityUserId,
            ExecutorIdentityUserId = sender.ExecutorIdentityUserId,
            StatusDocument = sender.StatusDocument,
            Name = sender.Name,
            CreatedAtUTC = sender.CreatedAtUTC,
            LastUpdateAt = sender.LastUpdateAt,
            Description = sender.Description,
            Id = sender.Id,
            ProjectId = sender.ProjectId,
            RubricIssue = sender.RubricIssue,
            Subscribers = sender.Subscribers,
        };
    }
}