////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IdentityPasswordTokenModel
/// </summary>
public class IdentityPasswordTokenModel
{
    /// <summary>
    /// UserId
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    public required string Token { get; set; }
}