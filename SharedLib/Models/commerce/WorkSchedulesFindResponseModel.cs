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
    public List<WeeklyScheduleModelDB> Schedules { get; set; } = default!;

    /// <summary>
    /// Calendars
    /// </summary>
    public List<CalendarScheduleModelDB> Calendars { get; set; } = default!;

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

        for (DateOnly dt = StartDate; dt <= EndDate; dt = dt.AddDays(1))
        {
            WeeklyScheduleModelDB[] weekly_sch = Schedules.Where(x => !x.IsDisabled && x.Weekday == dt.DayOfWeek).ToArray();
            CalendarScheduleModelDB[] calendar_sch = Calendars.Where(x => !x.IsDisabled && x.DateScheduleCalendar == dt).ToArray();

            if (weekly_sch.Length == 0 && calendar_sch.Length == 0)
                continue;

            if (calendar_sch.Length != 0)
            {
                foreach (CalendarScheduleModelDB csi in calendar_sch)
                {
                    foreach (OrganizationContractorModel oc in OrganizationsContracts)
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
                foreach (OrganizationContractorModel oc in OrganizationsContracts)
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

            //WorkSchedulesViewModel _el = new()
            //{
            //    Date = dt,                 
            //};

            //res.Add(_el);
        }

        return res;
    }
}