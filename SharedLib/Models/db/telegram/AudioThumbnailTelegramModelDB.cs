////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// AudioThumbnailTelegramModelDB
/// </summary>
public class AudioThumbnailTelegramModelDB : PhotoSizeTelegramModel
{
    /// <summary>
    /// AudioOwner
    /// </summary>
    public AudioTelegramModelDB? AudioOwner { get; set; }
    /// <summary>
    /// AudioOwner
    /// </summary>
    public int AudioOwnerId { get; set; }
}