////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Ошибка отправки сообщения TelegramBot
/// </summary>
public class ErrorSendingMessageTelegramBotModelDB : IdSwitchableModel
{
    /// <summary>
    /// CreatedAtUtc
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

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
    /// Признак того, что ошибка уже не актуальна
    /// </summary>
    /// <remarks>
    /// Например если администратор исследовал и решил проблему
    /// </remarks>
    public bool IsRead { get; set; }
}