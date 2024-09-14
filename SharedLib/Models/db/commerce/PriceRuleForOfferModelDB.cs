////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Правило ценообразования для торгового предложения
/// </summary>
/// <remarks>
/// Для расчёта скидки стоимости по вкладке (если <see cref="JointAggregateForOrderDocument"/> == false) или в общем по документу (если <see cref="JointAggregateForOrderDocument"/> == true)
/// </remarks>
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

    /// <summary>
    /// Общая агрегация количества по всему документу.
    /// </summary>
    /// <remarks>
    /// По умолчанию FALSE - учитывается количество локальной (одной) вкладки для одного адреса. Если TRUE - тогда количество учитывается в документе всего.
    /// </remarks>
    public bool JointAggregateForOrderDocument { get; set; }
}