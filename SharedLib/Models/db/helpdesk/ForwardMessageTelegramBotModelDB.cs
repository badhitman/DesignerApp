////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// ForwardMessageTelegramBotModelDB
/// </summary>
[Index(nameof(ResultMessageTelegramId))]
[Index(nameof(ResultMessageId))]
[Index(nameof(CreatedAtUtc))]
public class ForwardMessageTelegramBotModelDB : ForwardMessageTelegramBotModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Результат пересылки (Telegram id сообщения в чате назначения)
    /// </summary>
    public required int ResultMessageTelegramId { get; set; }

    /// <summary>
    /// Результат пересылки db:id
    /// </summary>
    public required int ResultMessageId { get; set; }

    /// <summary>
    /// Created At Utc
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Ответы на пересланный вопрос клиента
    /// </summary>
    public List<AnswerToForwardModelDB>? Answers { get; set; }
}