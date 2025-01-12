////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IdentityDetailsModel
/// </summary>
public class IdentityDetailsModel
{
    /// <summary>
    /// UserId
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// FirstName
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// LastName
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// PhoneNum
    /// </summary>
    public string? PhoneNum { get; set; }
}