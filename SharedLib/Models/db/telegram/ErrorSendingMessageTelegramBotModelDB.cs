////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Ошибка отправки сообщения TelegramBot
/// </summary>
[Index(nameof(ChatId))]
public class ErrorSendingMessageTelegramBotModelDB : IdSwitchableModel
{
    /// <summary>
    /// CreatedAtUtc
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Chat
    /// </summary>
    public long ChatId { get; set; }

    /// <summary>
    /// Имя типа исключения
    /// </summary>
    public required string? ExceptionTypeName { get; set; }

    /// <summary>
    /// Message (error)
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Признак того, что ошибка возникла не в процессе отправки, а в процессе изменения существующего сообщения Telegram
    /// </summary>
    public bool IsEditing { get; set; }
}