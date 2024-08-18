////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;

namespace SharedLib;

/// <summary>
/// This object represents a general file (as opposed to <see cref="PhotoSizeTelegramModelDB">photos</see>, <see cref="Voice">voice messages</see> and <see cref="AudioTelegramModelDB">audio files</see>).
/// </summary>
public class DocumentTelegramModelDB : FileBaseTelegramModel
{
    /// <summary>
    /// Optional. Original filename as defined by sender
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? FileName { get; set; }

    /// <summary>
    /// Optional. MIME type of the file as defined by sender
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? MimeType { get; set; }
}