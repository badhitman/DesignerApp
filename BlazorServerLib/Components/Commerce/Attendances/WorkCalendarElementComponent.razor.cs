////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// WorkCalendarElementComponent
/// </summary>
public partial class WorkCalendarElementComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Commerce
    /// </summary>
    [Inject]
    protected ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// WorkScheduleCalendar
    /// </summary>
    [Parameter, EditorRequired]
    public required WorkScheduleCalendarModelDB WorkScheduleCalendar { get; set; }


    WorkScheduleCalendarModelDB editWorkScheduleCalendar = default!;

    bool IsEdited => WorkScheduleCalendar.IsDisabled != editWorkScheduleCalendar.IsDisabled ||
        WorkScheduleCalendar.StartPart != editWorkScheduleCalendar.StartPart ||
        WorkScheduleCalendar.EndPart != editWorkScheduleCalendar.EndPart ||
        WorkScheduleCalendar.Name != editWorkScheduleCalendar.Name ||
        WorkScheduleCalendar.DateScheduleCalendar != editWorkScheduleCalendar.DateScheduleCalendar ||
        WorkScheduleCalendar.Description != editWorkScheduleCalendar.Description;

    async Task SaveScheduleCalendar()
    {
        await SetBusy();
        TResponseModel<int> res = await CommerceRepo.WorkScheduleCalendarUpdate(editWorkScheduleCalendar);
        WorkScheduleCalendar = GlobalTools.CreateDeepCopy(editWorkScheduleCalendar)!;
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        editWorkScheduleCalendar = GlobalTools.CreateDeepCopy(WorkScheduleCalendar)!;
    }
}