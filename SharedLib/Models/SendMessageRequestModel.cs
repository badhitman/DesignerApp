////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SendMessageRequestModel
/// </summary>
public class SendMessageRequestModel
{
    /// <summary>
    /// Body
    /// </summary>
    public required string Body { get; set; }

    /// <summary>
    /// Recipient
    /// </summary>
    public required string Recipient { get; set; }
}