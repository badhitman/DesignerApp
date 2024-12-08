////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;

namespace BlazorWebLib.Components.WorkScheduleWeekday;

/// <summary>
/// WorkScheduleWeekdayAddingComponent
/// </summary>
public partial class WorkScheduleWeekdayAddingComponent : BlazorBusyComponentBaseModel
{
    bool IsExpandAdding;


    async Task Save()
    {
        await SetBusy();

        await SetBusy(false);

        IsExpandAdding = !IsExpandAdding;
    }
}