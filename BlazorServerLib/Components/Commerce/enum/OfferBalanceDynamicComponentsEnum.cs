////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace BlazorWebLib;

/// <summary>
/// OfferBalanceDynamicComponentsEnum
/// </summary>
public enum OfferBalanceDynamicComponentsEnum
{
    /// <summary>
    /// <see cref="BlazorWebLib.Components.Commerce.GoodsOfferBalanceComponent"/>
    /// </summary>
    [Description(nameof(BlazorWebLib.Components.Commerce.GoodsOfferBalanceComponent))]
    Goods,

    /// <summary>
    /// <see cref="BlazorWebLib.Components.Commerce.Attendances.AttendancesOfferBalanceComponent"/>
    /// </summary>
    [Description(nameof(BlazorWebLib.Components.Commerce.Attendances.AttendancesOfferBalanceComponent))]
    Attendances,
}