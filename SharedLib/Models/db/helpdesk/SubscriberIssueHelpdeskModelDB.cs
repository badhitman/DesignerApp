////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// SubscriberIssueModelDB
/// </summary>
[Index(nameof(AuthorIdentityUserId), nameof(IssueId), IsUnique = true)]
public class SubscriberIssueHelpdeskModelDB : EntryModel
{
    /// <summary>
    /// Issue
    /// </summary>
    public int IssueId { get; set; }
    /// <summary>
    /// Issue
    /// </summary>
    public IssueHelpdeskModelDB? Issue { get; set; }

    /// <summary>
    /// IdentityUserId
    /// </summary>
    public required string AuthorIdentityUserId { get; set; }

    /// <summary>
    /// отключение отправки уведомлений
    /// </summary>
    public bool IsSilent { get; set; }
}