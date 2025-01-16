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
    public string? ViewMode {  get; set; }

    OfferBalanceDynamicComponentsEnum ModeView
        => string.IsNullOrWhiteSpace(ViewMode) || !Enum.TryParse(typeof(OfferBalanceDynamicComponentsEnum), ViewMode, out object? ovm)
        ? OfferBalanceDynamicComponentsEnum.Goods 
        : (OfferBalanceDynamicComponentsEnum)ovm;
}