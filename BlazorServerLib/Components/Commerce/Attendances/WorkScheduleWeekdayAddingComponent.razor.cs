////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// Добавление/Создание WorkSchedule для Weekday
/// </summary>
public partial class WorkScheduleWeekdayAddingComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// Weekday
    /// </summary>
    [Parameter, EditorRequired]
    public required DayOfWeek Weekday { get; set; }

    /// <summary>
    /// Создание/добавление нового WorkSchedule
    /// </summary>
    [Parameter]
    public required Action<WorkScheduleModelDB>? AddingWorkScheduleHandle { get; set; }


    bool IsExpandAdding;

    /// <summary>
    /// StartPart
    /// </summary>
    TimeSpan? StartPart = new(09, 00, 00);

    /// <summary>
    /// EndPart
    /// </summary>
    TimeSpan? EndPart = new(18, 00, 00);


    async Task Save()
    {
        if (EndPart is null || StartPart is null)
            return;

        WorkScheduleModelDB ws = new()
        {
            Name = "",
            EndPart = EndPart.Value,
            StartPart = StartPart.Value,
            Weekday = Weekday
        };

        await SetBusy();
        TResponseModel<int> res = await CommerceRepo.WorkScheduleUpdate(ws);
        ws.Id = res.Response;
        if (res.Success() && ws.Id != 0 && AddingWorkScheduleHandle is not null)
        {
            IsExpandAdding = !IsExpandAdding;
            AddingWorkScheduleHandle(ws);
        }
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }
}