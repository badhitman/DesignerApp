////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Commerce.Attendances;
using BlazorWebLib.Components.Commerce;
using System.ComponentModel;

namespace BlazorWebLib;

/// <summary>
/// OffersListModesEnum
/// </summary>
public enum OffersListModesEnum
{
    /// <summary>
    /// <see cref="OffersGoodsListComponent"/>
    /// </summary>
    [Description(nameof(OffersGoodsListComponent))]
    Goods,

    /// <summary>
    /// <see cref="OffersAttendancesListComponent"/>
    /// </summary>
    [Description(nameof(OffersAttendancesListComponent))]
    Attendances,
}