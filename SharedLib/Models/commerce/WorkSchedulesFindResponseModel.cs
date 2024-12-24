////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

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
    public List<WeeklyScheduleModelDB> WeeklySchedules { get; set; } = default!;

    /// <summary>
    /// Calendars
    /// </summary>
    public List<CalendarScheduleModelDB> CalendarsSchedules { get; set; } = default!;

    /// <summary>
    /// OrganizationsContracts
    /// </summary>
    public List<OrganizationContractorModel> OrganizationsContracts { get; set; } = default!;

    /// <summary>
    /// OrdersAttendances
    /// </summary>
    public List<OrderAnonModelDB> OrdersAttendances { get; set; } = default!;

    /// <summary>
    /// WorkSchedulesViews
    /// </summary>
    public List<WorkSchedulesViewModel> WorksSchedulesViews()
    {
        List<WorkSchedulesViewModel> res = [];
        if (OrganizationsContracts.Count == 0)
            return res;

        DateOnly _currentDate = DateOnly.FromDateTime(DateTime.Now);
        TimeSpan _currentTime = DateTime.Now.TimeOfDay;
        bool _is_current;
        for (DateOnly dt = StartDate; dt <= EndDate; dt = dt.AddDays(1))
        {
            _is_current = _currentDate.Equals(dt);

            WeeklyScheduleModelDB[] weekly_sch = WeeklySchedules
                .Where(x => !x.IsDisabled && x.Weekday == dt.DayOfWeek)
                .Where(x => !_is_current || x.StartPart >= _currentTime)
                .ToArray();

            CalendarScheduleModelDB[] calendar_sch = CalendarsSchedules
                .Where(x => !x.IsDisabled && x.DateScheduleCalendar == dt)
                .Where(x => !_is_current || x.StartPart >= _currentTime)
                .ToArray();

            if (weekly_sch.Length == 0 && calendar_sch.Length == 0)
                continue;

            OrganizationContractorModel[] organizations_query = OrganizationsContracts
                .OrderByDescending(x => !x.OfferId.HasValue || x.OfferId.Value < 1)
                .ToArray();

            if (calendar_sch.Length != 0)
            {
                foreach (CalendarScheduleModelDB csi in calendar_sch)
                {
                    foreach (OrganizationContractorModel oc in organizations_query)
                    {
                        WorkSchedulesViewModel _el = new()
                        {
                            Date = dt,
                            EndPart = csi.EndPart,
                            StartPart = csi.StartPart,
                            IsGlobalPermission = !oc.OfferId.HasValue || oc.OfferId.Value < 1,
                            Organization = oc.Organization!,
                            QueueCapacity = csi.QueueCapacity,
                        };

                        res.Add(_el);
                    }
                }
                continue;
            }

            foreach (WeeklyScheduleModelDB csi in weekly_sch)
            {
                foreach (OrganizationContractorModel oc in organizations_query)
                {
                    WorkSchedulesViewModel _el = new()
                    {
                        Date = dt,
                        EndPart = csi.EndPart,
                        StartPart = csi.StartPart,
                        IsGlobalPermission = !oc.OfferId.HasValue || oc.OfferId.Value < 1,
                        Organization = oc.Organization!,
                        QueueCapacity = csi.QueueCapacity,
                    };

                    res.Add(_el);
                }
            }
        }

        return res;
    }
}