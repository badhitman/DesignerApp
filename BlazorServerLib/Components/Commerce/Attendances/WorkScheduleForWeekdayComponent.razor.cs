////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// WorkScheduleForWeekdayComponent
/// </summary>
public partial class WorkScheduleForWeekdayComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// WorkSchedule
    /// </summary>
    [Parameter, EditorRequired]
    public required WorkScheduleModelDB WorkSchedule { get; set; }
}