////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// OfferGoodModelDB
/// </summary>
public class OfferGoodModelDB : EntrySwitchableUpdatedModel
{
    /// <summary>
    /// Good
    /// </summary>
    public GoodModelDB? Good { get; set; }
    /// <summary>
    /// Good
    /// </summary>
    public int GoodId { get; set; }

    /// <summary>
    /// Единица измерения предложения
    /// </summary>
    public UnitsOfMeasurementEnum OfferUnit { get; set; } = UnitsOfMeasurementEnum.Thing;

    /// <summary>
    /// Кратность к базовой единице товара
    /// </summary>
    public uint Multiplicity { get; set; }

    /// <summary>
    /// Цена за единицу <see cref="OfferUnit"/>
    /// </summary>
    public double Price { get; set; }
}