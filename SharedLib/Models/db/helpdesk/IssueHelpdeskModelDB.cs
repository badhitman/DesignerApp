////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// IssueTelegramModelDB
/// </summary>
[Index(nameof(AuthorIdentityUserId), nameof(ExecutorIdentityUserId), nameof(LastUpdateAt))]
public class IssueHelpdeskModelDB : IssueHelpdeskModel
{
    /// <inheritdoc/>
    public string? NormalizedNameUpper { get; set; }

    /// <summary>
    /// Rubric Issue
    /// </summary>
    public required int? RubricIssueId { get; set; }

    /// <summary>
    /// Subscribers
    /// </summary>
    public List<SubscriberIssueHelpdeskModelDB>? Subscribers { get; set; }

    /// <summary>
    /// Messages
    /// </summary>
    public List<IssueMessageHelpdeskModelDB>? Messages { get; set; }

    /// <summary>
    /// ReadMarkers
    /// </summary>
    public List<IssueReadMarkerHelpdeskModelDB>? ReadMarkers { get; set; }

    /// <summary>
    /// События
    /// </summary>
    public List<PulseIssueModelDB>? PulseEvents { get; set; }
}