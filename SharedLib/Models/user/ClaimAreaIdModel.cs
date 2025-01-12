////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ClaimAreaIdModel
/// </summary>
public class ClaimAreaIdModel
{
    /// <summary>
    /// ClaimArea
    /// </summary>
    public required ClaimAreasEnum ClaimArea { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    public required int Id { get; set; }
}