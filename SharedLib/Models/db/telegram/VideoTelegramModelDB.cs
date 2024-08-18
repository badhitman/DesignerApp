////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// This object represents a video file.
/// </summary>
public class VideoTelegramModelDB : FileBaseTelegramModel
{
    /// <summary>
    /// Video width as defined by sender
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Video height as defined by sender
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Duration of the video in seconds as defined by sender
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Optional. Original filename as defined by sender
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Optional. Mime type of a file as defined by sender
    /// </summary>
    public string? MimeType { get; set; }
}
