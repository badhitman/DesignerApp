////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;

namespace SharedLib;

/// <summary>
/// This object represents a voice note.
/// </summary>
public class VoiceTelegramModelDB : FileBaseTelegramModel
{
    /// <summary>
    /// Duration of the audio in seconds as defined by sender
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public int Duration { get; set; }

    /// <summary>
    /// Optional. MIME type of the file as defined by sender
    /// </summary>
    public string? MimeType { get; set; }
}