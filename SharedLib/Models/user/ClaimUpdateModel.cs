////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ClaimUpdateModel
/// </summary>
public class ClaimUpdateModel
{
    /// <summary>
    /// ClaimArea
    /// </summary>
    public required ClaimAreasEnum ClaimArea { get; set; }

    /// <summary>
    /// ClaimUpdate
    /// </summary>
    public required ClaimModel ClaimUpdate { get; set; }
}