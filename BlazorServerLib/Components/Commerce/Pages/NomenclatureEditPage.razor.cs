////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;

namespace BlazorWebLib.Components.Commerce.Pages;

/// <summary>
/// NomenclatureEditPage
/// </summary>
public partial class NomenclatureEditPage : BlazorBusyComponentBaseAuthModel
{
    /// <summary>
    /// NomenclatureId
    /// </summary>
    [Parameter]
    public int NomenclatureId { get; set; }

    /// <summary>
    /// ViewMode
    /// </summary>
    [Parameter]
    public OfferBalanceDynamicComponentsEnum ViewMode {  get; set; } = OfferBalanceDynamicComponentsEnum.Goods;
}