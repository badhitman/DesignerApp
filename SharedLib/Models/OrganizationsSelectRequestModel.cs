////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// OrganizationsSelectRequestModel
/// </summary>
public class OrganizationsSelectRequestModel : UniversalSelectRequestModel
{
    /// <summary>
    /// OffersFilter
    /// </summary>
    public int[]? OffersFilter { get; set; }
}