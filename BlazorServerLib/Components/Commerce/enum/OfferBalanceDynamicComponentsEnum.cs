////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Commerce.Attendances;
using BlazorWebLib.Components.Commerce;
using System.ComponentModel;

namespace BlazorWebLib;

/// <summary>
/// OfferBalanceDynamicComponentsEnum
/// </summary>
public enum OfferBalanceDynamicComponentsEnum
{
    /// <summary>
    /// <see cref="GoodsOfferBalanceComponent"/>
    /// </summary>
    [Description(nameof(GoodsOfferBalanceComponent))]
    Goods,

    /// <summary>
    /// <see cref="AttendancesOfferBalanceComponent"/>
    /// </summary>
    [Description(nameof(AttendancesOfferBalanceComponent))]
    Attendances,
}