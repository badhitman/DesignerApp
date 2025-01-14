////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// TelegramJoinAccountStateRequestModel
/// </summary>
public class TelegramJoinAccountStateRequestModel
{
    /// <summary>
    /// TelegramJoinAccountTokenLifetimeMinutes
    /// </summary>
    public uint TelegramJoinAccountTokenLifetimeMinutes { get; set; }

    /// <summary>
    /// EmailNotify
    /// </summary>
    public bool EmailNotify { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    public required string UserId { get; set; }
}