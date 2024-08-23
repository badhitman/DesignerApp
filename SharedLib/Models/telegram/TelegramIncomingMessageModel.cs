////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// TelegramIncomingMessageModel
/// </summary>
public class TelegramIncomingMessageModel : MessageTelegramModelDB
{
    /// <summary>
    /// User
    /// </summary>
    public required CheckTelegramUserAuthModel User { get; set; }
}