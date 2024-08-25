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
    public required long DestinationChatId { get; set; }

    /// <summary>
    /// Чат источник (Telegram id)
    /// </summary>
    public required long SourceChatId { get; set; }

    /// <summary>
    /// Пересылаемое сообщение (Telegram id)
    /// </summary>
    public required int SourceMessageId { get; set; }
}