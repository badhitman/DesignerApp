////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// WorkScheduleCalendarModelDB
/// </summary>
[Index(nameof(DateScheduleCalendar))]
public class WorkScheduleCalendarModelDB : WorkScheduleBaseModelDB
{
    /// <summary>
    /// Дата
    /// </summary>
    public required DateOnly DateScheduleCalendar { get; set; }
}