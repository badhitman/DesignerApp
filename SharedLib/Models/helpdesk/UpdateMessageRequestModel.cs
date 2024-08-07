////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// UpdateMessageRequest
/// </summary>
public class UpdateMessageRequestModel
{
    /// <summary>
    /// MessageId
    /// </summary>
    public required int MessageId { get; set; }

    /// <summary>
    /// MessageText (если NULL, тогда удаление сообщения)
    /// </summary>
    public required string? MessageText { get; set; }
}
