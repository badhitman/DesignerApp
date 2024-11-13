////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// RowOfOffersAvailabilityModelDB
/// </summary>
public class RowOfOffersAvailabilityModelDB : RowOfWarehouseDocumentAbstractModelDB
{
    /// <summary>
    /// Offer availability
    /// </summary>
    public OfferAvailabilityModelDB? OfferAvailability { get; set; }
    /// <summary>
    /// Offer availability
    /// </summary>
    public int OfferAvailabilityId { get; set; }
}