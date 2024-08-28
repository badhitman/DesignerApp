////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// This object represents one size of a photo or a <see cref="DocumentTelegramModelDB">file</see>.
/// </summary>
/// <remarks>A missing thumbnail for a file (or sticker) is presented as an empty object.</remarks>
public class PhotoSizeTelegramModel : FileBaseTelegramModel
{
    /// <summary>
    /// Photo width
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Photo height
    /// </summary>
    public int Height { get; set; }
}