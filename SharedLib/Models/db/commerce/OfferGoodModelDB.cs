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
    /// Goods
    /// </summary>
    public GoodsModelDB? Goods { get; set; }
    /// <summary>
    /// Good
    /// </summary>
    public int GoodsId { get; set; }

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
    public decimal Price { get; set; }

    /// <summary>
    /// Правила ценообразования
    /// </summary>
    public List<PriceRuleForOfferModelDB>? PricesRules { get; set; }

    /// <inheritdoc/>
    public static bool operator ==(OfferGoodModelDB off1, OfferGoodModelDB off2) => off1.Equals(off2);

    /// <inheritdoc/>
    public static bool operator !=(OfferGoodModelDB off1, OfferGoodModelDB off2)
    {
        return
                off1.Id != off2.Id ||
                off1.IsDisabled != off2.IsDisabled ||
                off1.GoodsId != off2.GoodsId ||
                off1.Name != off2.Name ||
                off1.Price != off2.Price ||
                off1.Multiplicity != off2.Multiplicity ||
                off1.OfferUnit != off2.OfferUnit;
    }

    /// <summary>
    /// GetName
    /// </summary>
    public string GetName => string.IsNullOrWhiteSpace(Name)
        ? $"{Goods?.Name} [{OfferUnit.DescriptionInfo().ToLower()}] (x{Multiplicity} {Goods?.BaseUnit.DescriptionInfo().ToLower()})"
        : Name;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is OfferGoodModelDB off)
            return
                off.IsDisabled == IsDisabled &&
                off.GoodsId == GoodsId &&
                off.Id == Id &&
                off.Name == Name &&
                off.Price == Price &&
                off.Multiplicity == Multiplicity &&
                off.OfferUnit == OfferUnit;

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{IsDisabled}{GoodsId}{Id}{Name}{Price}{Multiplicity}{OfferUnit}".GetHashCode();
    }
}