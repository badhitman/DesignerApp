////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// WorkScheduleComponent
/// </summary>
public partial class WorkScheduleComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Offer
    /// </summary>
    [Parameter, EditorRequired]
    public required OfferModelDB? Offer { get; set; }

    /// <summary>
    /// Reload
    /// </summary>
    public async Task Reload()
    {
        await SetBusy();

        await SetBusy(false);
    }
}