////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Данные для обработки входящего сообщения Telegram
/// </summary>
public class CheckTelegramUserBaseModel
{
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

    /// <summary>
    ///  true, if this user is a bot
    /// </summary>
    public bool IsBot { get; set; }
}