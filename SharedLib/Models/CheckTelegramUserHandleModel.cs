namespace SharedLib;

/// <summary>
/// Данные для обработки входящего сообщения Telegram
/// </summary>
public class CheckTelegramUserHandleModel
{
    /// <inheritdoc/>
    public static CheckTelegramUserHandleModel Build(long telegramUserId, string firstName, string? lastName, string? username, bool isBot)
        => new()
        {
            TelegramUserId = telegramUserId,
            FirstName = firstName,
            LastName = lastName,
            Username = username,
            IsBot = isBot,
        };

    /// <summary>
    /// Уникальный идентификатор Telegram пользователя (или бота)
    /// </summary>
    public long TelegramUserId { get; set; }

    /// <summary>
    ///  true, if this user is a bot
    /// </summary>
    public bool IsBot { get; set; }

    /// <summary>
    /// User's or bot’s first name
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Optional. User's or bot’s last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Optional. User's or bot’s username
    /// </summary>
    public string? Username { get; set; }
}