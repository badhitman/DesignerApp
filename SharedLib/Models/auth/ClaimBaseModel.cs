////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Claim
/// </summary>
public class ClaimBaseModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string? ClaimType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string? ClaimValue { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Equals(string? claimType, string? claimValue)
        => ClaimType?.Equals(claimType) == true && ClaimValue?.Equals(claimValue) == true;
}