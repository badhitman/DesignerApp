////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// VideoThumbnailTelegramModelDB
/// </summary>
public class VideoThumbnailTelegramModelDB : PhotoSizeTelegramModel
{
    /// <summary>
    /// AudioOwner
    /// </summary>
    public VideoTelegramModelDB? VideoOwner { get; set; }
    /// <summary>
    /// AudioOwner
    /// </summary>
    public int VideoOwnerId { get; set; }
}