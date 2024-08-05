////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Main message for user (Telegram)
/// </summary>
public class MainUserMessageModel
{
    /// <summary>
    /// Message Id (Telegram)
    /// </summary>
    public required int MessageId { get; set; }

    /// <summary>
    /// User Id (Telegram)
    /// </summary>
    public required long UserId { get; set; }
}