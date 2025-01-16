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

    OffersListModesEnum GetMode => string.IsNullOrWhiteSpace(ViewMode) || !Enum.TryParse(typeof(OffersListModesEnum), ViewMode, out object? pvm) ? OffersListModesEnum.Goods : (OffersListModesEnum)pvm;
}