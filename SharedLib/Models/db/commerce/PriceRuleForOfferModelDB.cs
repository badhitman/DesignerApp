////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// PriceRuleForOfferModelDB
/// </summary>
[Index(nameof(OfferId), nameof(QuantityRule), IsUnique = true)]
public class PriceRuleForOfferModelDB : EntrySwitchableUpdatedModel
{
    /// <summary>
    /// Offer
    /// </summary>
    public OfferGoodModelDB? Offer { get; set; }
    /// <summary>
    /// Offer
    /// </summary>
    public int OfferId { get; set; }

    /// <summary>
    /// Количество, начиная с которого срабатывает правило для цены <see cref="PriceRule"/>
    /// </summary>
    public uint QuantityRule { get; set; }

    /// <summary>
    /// Цена за единицу при срабатывании правила <see cref="QuantityRule"/>
    /// </summary>
    public decimal PriceRule { get; set; }
}