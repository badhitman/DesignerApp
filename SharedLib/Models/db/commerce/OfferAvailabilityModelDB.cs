////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// OfferAvailabilityModelDB
/// </summary>
[Index(nameof(WarehouseId))]
public class OfferAvailabilityModelDB : RowOfBaseDocumentModelDB
{
    /// <summary>
    /// Rubric
    /// </summary>
    public required int WarehouseId { get; set; }
}