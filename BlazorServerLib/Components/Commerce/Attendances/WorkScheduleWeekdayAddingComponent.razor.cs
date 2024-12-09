////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// WorkScheduleWeekdayAddingComponent
/// </summary>
public partial class WorkScheduleWeekdayAddingComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Weekday
    /// </summary>
    [Parameter, EditorRequired]
    public required DayOfWeek Weekday { get; set; }


    bool IsExpandAdding;


    async Task Save()
    {
        await SetBusy();

        await SetBusy(false);

        IsExpandAdding = !IsExpandAdding;
    }
}