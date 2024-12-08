////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// WorkCalendarAddDateComponent
/// </summary>
public partial class WorkCalendarAddDateComponent : BlazorBusyComponentBaseModel
{
    bool IsExpanded;
    private DateTime? _date = DateTime.Today;

    async Task Save()
    {
        await SetBusy();

        await SetBusy(false);
    }
}