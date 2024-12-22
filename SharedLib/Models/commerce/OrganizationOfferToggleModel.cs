////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// OrganizationOfferToggleModel
/// </summary>
public class OrganizationOfferToggleModel
{
    /// <summary>
    /// Organization
    /// </summary>
    public required int OrganizationId { get; set; }

    /// <summary>
    /// Offer
    /// </summary>
    public int? OfferId { get; set; }

    /// <summary>
    /// SetValue
    /// </summary>
    public bool SetValue { get; set; }
}