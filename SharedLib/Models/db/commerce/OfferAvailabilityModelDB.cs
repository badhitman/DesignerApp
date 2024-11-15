////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// OfferAvailabilityModelDB
/// </summary>
public class OfferAvailabilityModelDB : RowOfBaseDocumentModelDB
{
    /// <summary>
    /// Rubric
    /// </summary>
    public required int RubricId { get; set; }
}