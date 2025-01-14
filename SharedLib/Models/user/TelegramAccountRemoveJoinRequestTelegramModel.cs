////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// TelegramAccountRemoveJoinRequestModel
/// </summary>
public class TelegramAccountRemoveJoinRequestTelegramModel
{
    /// <summary>
    /// TelegramId
    /// </summary>
    public required long TelegramId { get; set; }

    /// <summary>
    /// ClearBaseUri
    /// </summary>
    public required string ClearBaseUri { get; set; }
}