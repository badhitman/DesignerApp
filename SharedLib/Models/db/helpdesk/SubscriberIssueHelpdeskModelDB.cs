////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// SubscriberIssueModelDB
/// </summary>
[Index(nameof(UserId), nameof(IssueId), IsUnique = true)]
public class SubscriberIssueHelpdeskModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public int IssueId { get; set; }
    /// <summary>
    /// Issue
    /// </summary>
    public IssueHelpdeskModelDB? Issue { get; set; }

    /// <summary>
    /// UserId (of Identity)
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// отключение отправки уведомлений
    /// </summary>
    public bool IsSilent { get; set; }
}