////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// WorkScheduleWeekdayComponent
/// </summary>
public partial class WorkScheduleWeekdayComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Weekday
    /// </summary>
    [Parameter, EditorRequired]
    public required DayOfWeek Weekday { get; set; }
}