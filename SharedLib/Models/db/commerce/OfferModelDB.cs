////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLib;

/// <summary>
/// Торговое предложение
/// </summary>
public class OfferModelDB : EntrySwitchableUpdatedModel
{
    /// <summary>
    /// Короткое название
    /// </summary>
    public string? ShortName { get; set; }

    /// <summary>
    /// Шаблон допустимых значений через пробел
    /// </summary>
    public string? QuantitiesTemplate { get; set; }

    /// <summary>
    /// Остатки
    /// </summary>
    public List<OfferAvailabilityModelDB>? Registers { get; set; }

    /// <summary>
    /// QuantitiesTemplateValidate
    /// </summary>
    [NotMapped]
    public bool QuantitiesTemplateValidate
    {
        get
        {
            if (string.IsNullOrWhiteSpace(QuantitiesTemplate))
                return true;

            return QuantitiesTemplate.SplitToDecimalList().Count != 0 && 
                string.Join(" ", QuantitiesTemplate.SplitToDecimalList()).Length == QuantitiesTemplate.Trim().Length;
        }
    }

    /// <summary>
    /// QuantitiesValues
    /// </summary>
    [NotMapped]
    public System.Collections.Immutable.ImmutableList<decimal>? QuantitiesValues
    {
        get
        {
            if (string.IsNullOrWhiteSpace(QuantitiesTemplate))
                return null;

            return QuantitiesTemplate.SplitToDecimalList();
        }
    }

    /// <summary>
    /// Номенклатура
    /// </summary>
    public NomenclatureModelDB? Nomenclature { get; set; }
    /// <summary>
    /// Nomenclature
    /// </summary>
    public int NomenclatureId { get; set; }

    /// <summary>
    /// Единица измерения предложения
    /// </summary>
    public UnitsOfMeasurementEnum OfferUnit { get; set; } = UnitsOfMeasurementEnum.Thing;

    /// <summary>
    /// Кратность к базовой единице товара
    /// </summary>
    public decimal Multiplicity { get; set; }

    /// <summary>
    /// Цена за единицу <see cref="OfferUnit"/>
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Правила ценообразования
    /// </summary>
    public List<PriceRuleForOfferModelDB>? PricesRules { get; set; }

    /// <inheritdoc/>
    public static bool operator ==(OfferModelDB off1, OfferModelDB off2) => off1.Equals(off2);

    /// <inheritdoc/>
    public static bool operator !=(OfferModelDB off1, OfferModelDB off2)
    {
        return
                off1.Id != off2.Id ||
                off1.IsDisabled != off2.IsDisabled ||
                off1.NomenclatureId != off2.NomenclatureId ||
                off1.Name != off2.Name ||
                off1.ShortName != off2.ShortName ||
                off1.QuantitiesTemplate != off2.QuantitiesTemplate ||
                off1.Price != off2.Price ||
                off1.Multiplicity != off2.Multiplicity ||
                off1.OfferUnit != off2.OfferUnit;
    }

    /// <summary>
    /// GetName
    /// </summary>
    public string GetName()
    {
        if (!string.IsNullOrWhiteSpace(ShortName))
            return ShortName;
        else if (string.IsNullOrWhiteSpace(Name))
            return $"{Nomenclature?.Name} [{OfferUnit.DescriptionInfo().ToLower()}] (x{Multiplicity} {Nomenclature?.BaseUnit.DescriptionInfo().ToLower()})";

        return Name;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is OfferModelDB off)
            return
                off.IsDisabled == IsDisabled &&
                off.NomenclatureId == NomenclatureId &&
                off.Id == Id &&
                off.Name == Name &&
                off.QuantitiesTemplate == QuantitiesTemplate &&
                off.ShortName == ShortName &&
                off.Price == Price &&
                off.Multiplicity == Multiplicity &&
                off.OfferUnit == OfferUnit;

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{IsDisabled}{NomenclatureId}{Id}{Name}({ShortName}){Price}{Multiplicity}{OfferUnit}[{QuantitiesTemplate}]".GetHashCode();
    }
}