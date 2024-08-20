////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// PhotoMessageTelegramModelDB
/// </summary>
public class PhotoMessageTelegramModelDB : PhotoSizeTelegramModel
{
    /// <summary>
    /// Message
    /// </summary>
    public MessageTelegramModelDB? Message { get; set; }
}