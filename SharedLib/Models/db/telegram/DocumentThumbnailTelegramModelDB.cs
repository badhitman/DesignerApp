////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// DocumentThumbnailTelegramModelDB
/// </summary>
public class DocumentThumbnailTelegramModelDB : PhotoSizeTelegramModel
{
    /// <summary>
    /// Document
    /// </summary>
    public DocumentTelegramModelDB? DocumentOwner { get; set; }
    /// <summary>
    /// Document
    /// </summary>
    public int DocumentOwnerId { get; set; }
}