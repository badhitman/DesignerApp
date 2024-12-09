////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// WorkScheduleModelDB
/// </summary>
[Index(nameof(Weekday))]
public class WorkScheduleModelDB : WorkScheduleBaseModelDB
{
    /// <summary>
    /// День недели
    /// </summary>
    public required DayOfWeek Weekday { get; set; }
}