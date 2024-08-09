////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Токен анонимного доступа к обращению (или для создания его)
/// </summary>
/// <remarks>
/// Используется для доступа из Telegram WebApp
/// </remarks>
[Index(nameof(TokenAccess), IsUnique = true)]
[Index(nameof(IssueId), nameof(TelegramUserId), IsUnique = true)]
[Index(nameof(CreatedAt))]
public class AnonymTelegramAccessHelpdeskModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// GUID: TokenAccess
    /// </summary>
    [Required]
    public required string TokenAccess { get; set; }

    /// <summary>
    /// Телеграм пользователь, который создал токен
    /// </summary>
    [Required]
    public required long TelegramUserId { get; set; }

    /// <summary>
    /// CreatedAt
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Issue
    /// </summary>
    public IssueHelpdeskModelDB? Issue { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public int? IssueId { get; set; }
}