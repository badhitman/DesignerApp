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
    [CascadingParameter, EditorRequired]
    public required OfferModelDB? Offer { get; set; }


    WorkSchedulesOfWeekdayComponent? WorkSchedulesOfWeekday_ref;

    /// <summary>
    /// Reload
    /// </summary>
    public async Task Reload()
    {
        if (WorkSchedulesOfWeekday_ref is null)
            return;

        await SetBusy();
        await WorkSchedulesOfWeekday_ref.LoadData(0);
        await SetBusy(false);
    }
}