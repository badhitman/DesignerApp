////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// WorkSchedulesFindRequestModel
/// </summary>
public class WorkSchedulesFindRequestModel : WorkSchedulesFindBaseModel
{
    /// <summary>
    /// Имя контекста для разделения различных селекторов независимо друг от друга
    /// </summary>
    public string? ContextName { get; set; }

    /// <summary>
    /// Offers
    /// </summary>
    public int[]? OffersFilter { get; set; }
}

/// <summary>
/// WorkSchedulesFindResponseModel
/// </summary>
public class WorkSchedulesFindResponseModel : WorkSchedulesFindBaseModel
{
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