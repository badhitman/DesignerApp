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

    bool IsEdited => WorkSchedule.IsDisabled != workScheduleEdit.IsDisabled || 
        WorkSchedule.StartPart != workScheduleEdit.StartPart ||
        WorkSchedule.EndPart != workScheduleEdit.EndPart ||
        WorkSchedule.Name != workScheduleEdit.Name ||
        WorkSchedule.Description != workScheduleEdit.Description
        ;

    WorkScheduleModelDB workScheduleEdit { get; set; } = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        workScheduleEdit = GlobalTools.CreateDeepCopy(WorkSchedule)!;
    }
}