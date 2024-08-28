////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Подписчик на собыия в обращении (ModelDB)
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
    /// Обращение клиента с вопросом
    /// </summary>
    public int IssueId { get; set; }
    /// <summary>
    /// Обращение клиента с вопросом
    /// </summary>
    public IssueHelpdeskModelDB? Issue { get; set; }

    /// <summary>
    /// Пользователь, который подписан (of Identity)
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// отключение отправки уведомлений
    /// </summary>
    public bool IsSilent { get; set; }
}