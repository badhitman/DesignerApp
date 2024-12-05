////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Правило ценообразования для торгового предложения (для расчёта скидки по документу)
/// </summary>
[Index(nameof(OfferId), nameof(QuantityRule), IsUnique = true)]
public class PriceRuleForOfferModelDB : EntrySwitchableUpdatedModel
{
    /// <summary>
    /// Offer
    /// </summary>
    public OfferModelDB? Offer { get; set; }
    /// <summary>
    /// Offer
    /// </summary>
    public int OfferId { get; set; }

    /// <summary>
    /// Количество, начиная с которого срабатывает правило для цены <see cref="PriceRule"/>
    /// </summary>
    public decimal QuantityRule { get; set; }

    /// <summary>
    /// Цена за единицу при срабатывании правила <see cref="QuantityRule"/>
    /// </summary>
    public decimal PriceRule { get; set; }

    /// <inheritdoc/>
    public static PriceRuleForOfferModelDB Build(string name, int quantity, decimal price, int offer_id)
    {
        return new PriceRuleForOfferModelDB()
        {
            Name = name,
            CreatedAtUTC = DateTime.UtcNow,
            OfferId = offer_id,
            PriceRule = price,
            QuantityRule = (uint)quantity,
        };
    }
}