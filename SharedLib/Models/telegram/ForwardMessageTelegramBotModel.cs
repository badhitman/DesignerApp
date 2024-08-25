////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Пересылка сообщения
/// </summary>
[Index(nameof(DestinationChatId), nameof(SourceChatId), nameof(SourceMessageId))]
public class ForwardMessageTelegramBotModel
{
    /// <summary>
    /// Чат назначения (Telegram id)
    /// </summary>
    /// <remarks>
    /// В этот чат будет отправлено сообщение
    /// </remarks>
    public required long DestinationChatId { get; set; }

    /// <summary>
    /// Чат источник (Telegram id)
    /// </summary>
    /// <remarks>
    /// Исходный чат, из которого происходит пересылка сообщения
    /// </remarks>
    public required long SourceChatId { get; set; }

    /// <summary>
    /// Пересылаемое сообщение (Telegram id)
    /// </summary>
    /// <remarks>
    /// Исходное сообщение, которое пересылается
    /// </remarks>
    public required int SourceMessageId { get; set; }
}