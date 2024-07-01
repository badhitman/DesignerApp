////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Telegram dialog request
/// </summary>
public class TelegramDialogRequestModel
{
    /// <summary>
    /// Пользователь (отправитель  сообщения в Telegram)
    /// </summary>
    public required CheckTelegramUserModel TelegramUser { get; set; }

    /// <summary>
    /// Telegram MessageId
    /// </summary>
    public int MessageTelegramId { get; set; }

    /// <summary>
    /// Telegram Message text
    /// </summary>
    public required string MessageText { get; set; }

    /// <summary>
    /// Тип входящего сообщения
    /// </summary>
    public required MessagesTypesEnum TypeMessage { get; set; }
}