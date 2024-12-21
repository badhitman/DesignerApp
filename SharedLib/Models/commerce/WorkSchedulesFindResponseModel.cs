////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/*
 WorkSchedulesFindResponseModel
 */

/// <summary>
/// WorkSchedulesFindResponseModel
/// </summary>
public class WorkSchedulesFindResponseModel : WorkSchedulesFindBaseModel
{
    /// <summary>
    /// WorkSchedulesFindResponseModel
    /// </summary>
    public WorkSchedulesFindResponseModel(DateOnly start, DateOnly end)
    {
        StartDate = start;
        EndDate = end;
    }

    /// <summary>
    /// Schedules
    /// </summary>
    public WorkScheduleModelDB[]? Schedules { get; set; }

    /// <summary>
    /// Calendars
    /// </summary>
    public WorkScheduleCalendarModelDB[]? Calendars { get; set; }

    /// <summary>
    /// WorkSchedulesViews
    /// </summary>
    public WorkSchedulesViewModel[] WorkSchedulesViews() => [];
}