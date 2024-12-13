////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
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
    TimeSpan? StartPart;

    /// <summary>
    /// EndPart
    /// </summary>
    TimeSpan? EndPart;

    string? NameValue;


    async Task Save()
    {
        if (EndPart is null || StartPart is null || string.IsNullOrWhiteSpace(NameValue))
            return;

        await SetBusy();
        var res = await CommerceRepo.WorkScheduleUpdate(new WorkScheduleModelDB() { Name = NameValue, EndPart = EndPart.Value, StartPart = StartPart.Value, Weekday = Weekday });
        await SetBusy(false);

        IsExpandAdding = !IsExpandAdding;
    }
}