////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Доступные слоты для записи/брони
/// </summary>
public class WorksFindResponseModel : WorksFindBaseModel
{
    /// <summary>
    /// Доступные слоты для записи/брони
    /// </summary>
    public WorksFindResponseModel()
    {
        StartDate = DateOnly.FromDateTime(DateTime.MinValue);
        EndDate = DateOnly.FromDateTime(DateTime.MinValue);
    }

    /// <summary>
    /// Доступные слоты для записи/брони
    /// </summary>
    public WorksFindResponseModel(DateOnly start, DateOnly end, List<WeeklyScheduleModelDB> WeeklySchedules, List<CalendarScheduleModelDB> CalendarsSchedules, List<OrganizationContractorModel> OrganizationsContracts, List<RecordsAttendanceModelDB> OrdersAttendances)
    {
        StartDate = start;
        EndDate = end;

        if (OrganizationsContracts is null)
            return;

        if (OrganizationsContracts.Count == 0)
            return;

        DateOnly _currentDate = DateOnly.FromDateTime(DateTime.Now);
        TimeSpan _currentTime = DateTime.Now.TimeOfDay;

        bool _is_current;
        for (DateOnly dt = StartDate; dt <= EndDate; dt = dt.AddDays(1))
        {
            _is_current = _currentDate.Equals(dt);

            WeeklyScheduleModelDB[] weekly_sch = WeeklySchedules
                .Where(x => !x.IsDisabled && x.Weekday == dt.DayOfWeek)
                .Where(x => !_is_current || x.StartPart >= _currentTime || x.EndPart >= _currentTime)
                .ToArray();

            CalendarScheduleModelDB[] calendar_sch = CalendarsSchedules
                .Where(x => !x.IsDisabled && x.DateScheduleCalendar == dt)
               .Where(x => !_is_current || x.StartPart >= _currentTime || x.EndPart >= _currentTime)
                .ToArray();

            if (weekly_sch.Length == 0 && calendar_sch.Length == 0)
                continue;

            OrganizationContractorModel[] organizations_query = [.. OrganizationsContracts
                .OrderByDescending(x => !x.OfferId.HasValue || x.OfferId.Value < 1)];

            if (calendar_sch.Length != 0)
            {
                foreach (CalendarScheduleModelDB csi in calendar_sch)
                {
                    foreach (OrganizationContractorModel oc in organizations_query)
                    {
                        if (WorksSchedulesViews.Any(x => x.Date == dt && x.StartPart == csi.StartPart && x.EndPart == csi.EndPart && x.Organization.Id == oc.Organization!.Id))
                            continue;

                        WorkScheduleModel _el = new()
                        {
                            IsGlobalPermission = !oc.OfferId.HasValue || oc.OfferId.Value < 1,
                            IsStarted = _is_current && csi.StartPart <= _currentTime,
                            QueueCapacity = csi.QueueCapacity,
                            Organization = oc.Organization!,
                            StartPart = csi.StartPart,
                            EndPart = csi.EndPart,
                            Date = dt,
                        };

                        WorksSchedulesViews.Add(_el);
                    }
                }
                continue;
            }

            foreach (WeeklyScheduleModelDB csi in weekly_sch)
            {
                foreach (OrganizationContractorModel oc in organizations_query)
                {
                    if (WorksSchedulesViews.Any(x => x.Date == dt && x.StartPart == csi.StartPart && x.EndPart == csi.EndPart && x.Organization.Id == oc.Organization!.Id))
                        continue;

                    WorkScheduleModel _el = new()
                    {
                        IsGlobalPermission = !oc.OfferId.HasValue || oc.OfferId.Value < 1,
                        IsStarted = _is_current && csi.StartPart <= _currentTime,
                        QueueCapacity = csi.QueueCapacity,
                        Organization = oc.Organization!,
                        StartPart = csi.StartPart,
                        EndPart = csi.EndPart,
                        Date = dt,
                    };

                    WorksSchedulesViews.Add(_el);
                }
            }
        }

        static bool QueryFindIndex(RecordsAttendanceModelDB order, WorkScheduleModel ws)
        {
            return
                order.DateExecute == ws.Date &&
                order.OrganizationId == ws.Organization.Id &&
                order.StartPart.ToTimeSpan() == ws.StartPart &&
                order.EndPart.ToTimeSpan() == ws.EndPart;
        }

        if (OrdersAttendances is not null && OrdersAttendances.Count != 0)
            OrdersAttendances.ForEach(x =>
            {
                int _i = WorksSchedulesViews.FindIndex(y => QueryFindIndex(x, y));

                if (_i == -1)
                    return;

                if (WorksSchedulesViews[_i].QueueCapacity > 0)
                    WorksSchedulesViews[_i].QueueCapacity--;
                else if (WorksSchedulesViews[_i].QueueCapacity == 1)
                    WorksSchedulesViews.RemoveAt(_i);
            });
    }

    /// <summary>
    /// WorkSchedulesViews
    /// </summary>
    public List<WorkScheduleModel> WorksSchedulesViews { get; set; } = [];
}