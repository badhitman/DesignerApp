////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// MarkAsResponseModelDB
/// </summary>
[Index(nameof(TelegramId))]
[Index(nameof(IdentityUserId))]
public class MarkAsResponseHelpdeskModelDB : UserCrossIdsModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ParentMessage
    /// </summary>
    public IssueMessageHelpdeskModelDB? Message { get; set; }
    /// <summary>
    /// ParentMessage
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public IssueHelpdeskModelDB? Issue { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public int IssueId { get; set; }
}