////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// ForwardMessageTelegramBotModelDB
/// </summary>
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
}