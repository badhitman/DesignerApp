////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// LockOffersAvailabilityModelDB
/// </summary>
[Index(nameof(LockerId), nameof(LockerName), IsUnique = true)]
public class LockOffersAvailabilityModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// LockerName
    /// </summary>
    public required string LockerName { get; set; }

    /// <summary>
    /// LockerId
    /// </summary>
    public int LockerId { get; set; }
}
