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
    public required CalendarScheduleModelDB WorkScheduleCalendar { get; set; }

    /// <summary>
    /// WorkCalendarReloadDateHandler
    /// </summary>
    [Parameter]
    public Action? WorkCalendarReloadDateHandler { get; set; }

    CalendarScheduleModelDB editWorkScheduleCalendar = default!;
    DateTime WorkScheduleDate
    {
        get => editWorkScheduleCalendar.DateScheduleCalendar.ToDateTime(new TimeOnly(0, 0, 0));
        set
        {
            editWorkScheduleCalendar.DateScheduleCalendar = DateOnly.FromDateTime(value);
        }
    }

    bool IsEdited => WorkScheduleCalendar.IsDisabled != editWorkScheduleCalendar.IsDisabled ||
        WorkScheduleCalendar.StartPart != editWorkScheduleCalendar.StartPart ||
        WorkScheduleCalendar.EndPart != editWorkScheduleCalendar.EndPart ||
        WorkScheduleCalendar.QueueCapacity != editWorkScheduleCalendar.QueueCapacity ||
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
        if (WorkCalendarReloadDateHandler is not null)
            WorkCalendarReloadDateHandler();
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        editWorkScheduleCalendar = GlobalTools.CreateDeepCopy(WorkScheduleCalendar)!;
    }
}