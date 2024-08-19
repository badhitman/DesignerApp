////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Ошибка отправки сообщения TelegramBot
/// </summary>
public class ErrorSendingTextMessageTelegramBotModelDB : IdSwitchableModel
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
    /// Message (error)
    /// </summary>
    public required string Message { get; set; }
}
