////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// PulseRequestModel
/// </summary>
public class PulseRequestModel
{
    /// <summary>
    /// Payload
    /// </summary>
    public required TAuthRequestModel<PulseIssueBaseModel> Payload { get; set; }

    /// <summary>
    /// IsMuteEmail
    /// </summary>
    public bool IsMuteEmail { get; set; }

    /// <summary>
    /// IsMuteTelegram
    /// </summary>
    public bool IsMuteTelegram { get; set; }
}