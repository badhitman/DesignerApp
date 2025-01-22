namespace SharedLib;

/// <summary>
/// Check Token 2FA Request
/// </summary>
public class CheckToken2FARequestModel
{
    /// <summary>
    /// UserAlias
    /// </summary>
    public required string UserAlias { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    public required string Token { get; set; }
}