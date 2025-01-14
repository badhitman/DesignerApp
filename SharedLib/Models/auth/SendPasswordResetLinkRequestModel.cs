////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SendPasswordResetLinkRequestModel
/// </summary>
public class SendPasswordResetLinkRequestModel
{
    /// <summary>
    /// BaseAddress
    /// </summary>
    public required string BaseAddress { get; set; }

    /// <summary>
    /// ResetToken
    /// </summary>
    public required string ResetToken { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    public required string UserId { get; set; }
}