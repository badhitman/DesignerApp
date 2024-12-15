////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// WorkCalendarComponent
/// </summary>
public partial class WorkCalendarComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Offer
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required OfferModelDB? Offer { get; set; }


    private int _selected = 11;

    /// <summary>
    /// Reload
    /// </summary>
    public async Task Reload()
    {
        await SetBusy();

        await SetBusy(false);
    }
}