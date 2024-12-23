////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// OrderAnonModelDB
/// </summary>
public class OrderAnonModelDB
{
    /// <summary>
    /// Торговое предложение
    /// </summary>
    [Required]
    public required OfferModelDB Offer { get; set; }

    /// <summary>
    /// DateExecute
    /// </summary>
    [Required]
    public required DateOnly DateExecute { get; set; }

    /// <summary>
    /// StartPart
    /// </summary>
    [Required]
    public required TimeOnly StartPart { get; set; }

    /// <summary>
    /// EndPart
    /// </summary>
    [Required]
    public required TimeOnly EndPart { get; set; }
}