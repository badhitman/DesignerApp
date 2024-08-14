////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Маркировка сообщения как `ответ на вопрос`
/// </summary>
[Index(nameof(IdentityUserId))]
public class VoteHelpdeskModelDB
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

    /// <summary>
    /// IdentityUserId
    /// </summary>
    [Required]
    public required string IdentityUserId { get; set; }
}