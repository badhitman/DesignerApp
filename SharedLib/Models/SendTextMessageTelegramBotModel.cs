namespace SharedLib;

/// <summary>
/// Отправка текстового сообщения пользователю через TelegramBot
/// </summary>
public class SendTextMessageTelegramBotModel
{
    /// <summary>
    /// Получатель сообщения
    /// </summary>
    public required TelegramUserBaseModelDb UserTelegram { get; set; }

    /// <summary>
    /// Текст сообщения
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Имя режима парсинга сообщений Telegram (default: Html). Пример: Html, Markdown, MarkdownV2
    /// <a href="https://core.telegram.org/bots/api#formatting-options"/>
    /// </summary>
    public string ParseModeName { get; set; } = "Html";

    /// <summary>
    /// Подпись (от кого сообщение). Если null, то подписи не будет
    /// </summary>
    public string? From { get; set; }

    /// <summary>
    /// Если сообщение является ответом, идентификатор исходного сообщения
    /// </summary>
    public int? ReplyToMessageId { get; set; }
}