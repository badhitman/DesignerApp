////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// LockOffersAvailabilityModelDB
/// </summary>
[Index(nameof(OfferAvailabilityId), IsUnique = true)]
public class LockOffersAvailabilityModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// OfferAvailability
    /// </summary>
    public OfferAvailabilityModelDB? OfferAvailability { get; set; }

    /// <summary>
    /// OfferAvailability
    /// </summary>
    public int OfferAvailabilityId { get; set; }
}
