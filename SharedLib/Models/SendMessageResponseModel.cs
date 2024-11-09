////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Text.Json.Serialization;

namespace SharedLib;

/// <summary>
/// SendMessageResponseModel
/// </summary>
public class SendMessageResponseModel
{
    /// <summary>
    /// Status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Timestamp
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// Time
    /// </summary>
    public string? Time { get; set; }

    /// <summary>
    /// MessageId
    /// </summary>
    [JsonPropertyName("message_id")]
    public string? MessageId { get; set; }

    /// <summary>
    /// TaskId
    /// </summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>
    /// Uuid
    /// </summary>
    public string? Uuid { get; set; }
}