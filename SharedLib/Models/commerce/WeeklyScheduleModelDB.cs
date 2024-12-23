////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// WeeklyScheduleModelDB
/// </summary>
public class WeeklyScheduleModelDB : WorkSchedulesFindBaseModel
{
    /// <summary>
    /// WeeklyScheduleModelDB
    /// </summary>
    public WeeklyScheduleModelDB(DateOnly start, DateOnly end)
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
    /// OrganizationsContracts
    /// </summary>
    public OrganizationContractorModel[]? OrganizationsContracts { get; set; }

    /// <summary>
    /// OrdersAttendances
    /// </summary>
    public OrderAnonModelDB[]? OrdersAttendances { get; set; }

    /// <summary>
    /// WorkSchedulesViews
    /// </summary>
    public List<WorkSchedulesViewModel> WorkSchedulesViews()
    {
        List<WorkSchedulesViewModel> res = [];
        
        for (DateOnly dt = StartDate; dt <= EndDate; dt = dt.AddDays(1))
        {
            WorkScheduleModelDB[]? w_sch = Schedules?.Where(x=>x.Weekday == dt.DayOfWeek).ToArray();
            WorkScheduleCalendarModelDB[]? d_sch = Calendars?.Where(x=>x.DateScheduleCalendar == dt).ToArray();

            //WorkSchedulesViewModel _el = new()
            //{
            //    Date = dt,                 
            //};

            //res.Add(_el);
        }

        return res;
    }
}