////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ClaimAreaOwnerModel
/// </summary>
public class ClaimAreaOwnerModel
{
    /// <summary>
    /// ClaimArea
    /// </summary>
    public required ClaimAreasEnum ClaimArea { get; set; }

    /// <summary>
    /// OwnerId
    /// </summary>
    public required string OwnerId { get; set; }
}