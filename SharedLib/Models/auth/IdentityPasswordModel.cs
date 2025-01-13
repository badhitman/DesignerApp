////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IdentityPasswordModel
/// </summary>
public class IdentityPasswordModel
{
    /// <summary>
    /// UserId
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public required string Password { get; set; }
}