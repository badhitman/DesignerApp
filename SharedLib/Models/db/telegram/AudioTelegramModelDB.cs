////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// This object represents an audio file to be treated as music by the Telegram clients.
/// </summary>
public class AudioTelegramModelDB : FileBaseTelegramModel
{
    /// <summary>
    /// Duration of the audio in seconds as defined by sender
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Optional. Performer of the audio as defined by sender or by audio tags
    /// </summary>
    public string? Performer { get; set; }

    /// <summary>
    /// Optional. Title of the audio as defined by sender or by audio tags
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Optional. Original filename as defined by sender
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Optional. MIME type of the file as defined by sender
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// AudioThumbnail
    /// </summary>
    public AudioThumbnailTelegramModelDB? AudioThumbnail { get; set; }
}