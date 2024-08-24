////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Пересылка сообщения
/// </summary>
public class ForwardMessageTelegramBotModel
{
    /// <summary>
    /// Чат назначения
    /// </summary>
    public required long DestinationChatId { get; set; }

    /// <summary>
    /// Чат источник
    /// </summary>
    public required long SourceChatId { get; set; }

    /// <summary>
    /// Пересылаемое сообщение
    /// </summary>
    public required int SourceMessageId { get; set; }
}