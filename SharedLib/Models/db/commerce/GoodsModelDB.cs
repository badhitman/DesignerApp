////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Номенклатура
/// </summary>
public class GoodsModelDB : EntrySwitchableUpdatedModel
{
    /// <summary>
    /// Базовая единица измерения `Номенклатуры`
    /// </summary>
    public UnitsOfMeasurementEnum BaseUnit { get; set; } = UnitsOfMeasurementEnum.Thing;

    /// <summary>
    /// Торговые предложения по Номенклатуре
    /// </summary>
    public List<OfferGoodModelDB>? Offers { get; set; }

    /// <summary>
    /// Остатки
    /// </summary>
    public List<OfferAvailabilityModelDB>? Registers { get; set; }


    /// <inheritdoc/>
    public static bool operator ==(GoodsModelDB off1, GoodsModelDB off2) => off1.Equals(off2);

    /// <inheritdoc/>
    public static bool operator !=(GoodsModelDB off1, GoodsModelDB off2)
    {
        return
                off1.Id != off2.Id ||
                off1.IsDisabled != off2.IsDisabled ||
                off1.Name != off2.Name ||
                off1.BaseUnit != off2.BaseUnit;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is GoodsModelDB off)
            return
                off.IsDisabled == IsDisabled &&
                off.Id == Id &&
                off.Name == Name &&
                off.BaseUnit == BaseUnit;

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{IsDisabled}{Id}{Name}{BaseUnit}".GetHashCode();
    }
}