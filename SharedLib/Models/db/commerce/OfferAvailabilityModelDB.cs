////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// OfferAvailabilityModelDB
/// </summary>
[Index(nameof(WarehouseId), nameof(OfferId), IsUnique = true)]
public class OfferAvailabilityModelDB : RowOfBaseDocumentModelDB
{
    /// <summary>
    /// Склад
    /// </summary>
    public required int WarehouseId { get; set; }
}