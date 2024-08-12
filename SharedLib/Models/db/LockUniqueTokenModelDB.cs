////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Блокировщик произвольных строковых токенов
/// </summary>
/// <remarks>
/// Идея в том, что следом за инициализацией транзакции можно попытаться добавить в этй таблицу какой-то строковой токен,
/// что бы исключить возможность создания подобного токена из другого места.
/// По завершении своей транзакции нужно не забыть удалить строку из этой таблицы, что бы этот токен стал доступным другим потокам.
/// </remarks>
[Index(nameof(Token), IsUnique = true)]
public class LockUniqueTokenModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    [Required]
    public required string Token { get; set; }
}