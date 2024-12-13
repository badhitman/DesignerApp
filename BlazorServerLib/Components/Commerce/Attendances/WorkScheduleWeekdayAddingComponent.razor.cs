////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// WorkScheduleWeekdayAddingComponent
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


    bool IsExpandAdding;

    /// <summary>
    /// StartPart
    /// </summary>
    TimeSpan? StartPart = new TimeSpan(09, 00, 00);

    /// <summary>
    /// EndPart
    /// </summary>
    TimeSpan? EndPart = new TimeSpan(18, 00, 00);


    async Task Save()
    {
        if (EndPart is null || StartPart is null)
            return;

        await SetBusy();
        TResponseModel<int> res = await CommerceRepo.WorkScheduleUpdate(new WorkScheduleModelDB() { Name = "", EndPart = EndPart.Value, StartPart = StartPart.Value, Weekday = Weekday });
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        IsExpandAdding = !IsExpandAdding;
    }
}