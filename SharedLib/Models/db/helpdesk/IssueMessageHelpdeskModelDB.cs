////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// IssueMessageModelDB
/// </summary>
[Index(nameof(AuthorUserId))]
[Index(nameof(CreatedAt), nameof(LastUpdateAt))]
public class IssueMessageHelpdeskModelDB : IssueMessageHelpdeskBaseModel
{
    /// <summary>
    /// Автор сообщения
    /// </summary>
    public required string AuthorUserId { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public IssueHelpdeskModelDB? Issue { get; set; }

    /// <summary>
    /// CreatedAt
    /// </summary>
    public required DateTime CreatedAt { get; set; }

    /// <summary>
    /// LastUpdateAt
    /// </summary>
    public required DateTime LastUpdateAt { get; set; }

    /// <summary>
    /// Отметки как ответ
    /// </summary>
    public List<VoteHelpdeskModelDB>? Votes { get; set; }
}