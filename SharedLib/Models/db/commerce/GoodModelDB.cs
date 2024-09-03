////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Товар
/// </summary>
public class GoodModelDB : EntrySwitchableUpdatedModel
{
    /// <summary>
    /// Базовая единица измерения
    /// </summary>
    public UnitsOfMeasurementEnum BaseUnit { get; set; } = UnitsOfMeasurementEnum.Thing;

    /// <summary>
    /// Products offers
    /// </summary>
    public List<OfferGoodModelDB>? ProductsOffers { get; set; }
}