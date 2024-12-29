////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Данные для обработки входящего сообщения Telegram
/// </summary>
public class CheckTelegramUserHandleModel : CheckTelegramUserBaseModel
{
    /// <summary>
    /// Уникальный идентификатор Telegram пользователя (или бота)
    /// </summary>
    public long TelegramUserId { get; set; }

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
}