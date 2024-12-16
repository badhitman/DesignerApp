////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// WorkScheduleComponent
/// </summary>
public partial class WorkScheduleComponent : BlazorBusyComponentBaseModel
{
    WorkSchedulesOfWeekdayComponent? WorkSchedulesOfWeekday_ref;

    /// <summary>
    /// Reload OfferModelDB selectedOffer
    /// </summary>
    public async Task Reload(OfferModelDB? selectedOffer)
    {
        await SetBusy();
        if (WorkSchedulesOfWeekday_ref is not null)
            await WorkSchedulesOfWeekday_ref.LoadData(0, selectedOffer);
        await SetBusy(false);
    }
}