////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SendEmailRequestModel
/// </summary>
public class SendEmailRequestModel
{
    /// <summary>
    /// Email
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Subject
    /// </summary>
    public required string Subject { get; set; }

    /// <summary>
    /// TextMessage
    /// </summary>
    public required string TextMessage { get; set; }

    /// <summary>
    /// MimeType
    /// </summary>
    public string MimeType { get; set; } = "html";
}