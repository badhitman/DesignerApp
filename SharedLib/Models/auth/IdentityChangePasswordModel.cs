namespace SharedLib;

/// <summary>
/// IdentitySetNewPasswordModel
/// </summary>
public class IdentityChangePasswordModel
{
    /// <summary>
    /// UserId
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// CurrentPassword
    /// </summary>
    public required string CurrentPassword { get; set; }

    /// <summary>
    /// NewPassword
    /// </summary>
    public required string NewPassword { get; set; }
}