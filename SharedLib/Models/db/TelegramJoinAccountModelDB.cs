using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Процедура привязки Telegram аккаунта к учётной записи сайта
/// </summary>
public class TelegramJoinAccountModelDb
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Identity id of user
    /// </summary>
    public required string UserIdentityId { get; set; }

    /// <summary>
    /// Токен подтверждения текущего шага процедуры привязки Telegram аккаунта к учётной записи сайта
    /// </summary>
    public required string GuidToken { get; set; }

    /// <summary>
    /// Дата/время последнего действия с токеном
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}