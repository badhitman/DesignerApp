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
    public WeeklyScheduleModelDB[] Schedules { get; set; } = default!;

    /// <summary>
    /// Calendars
    /// </summary>
    public CalendarScheduleModelDB[] Calendars { get; set; } = default!;

    /// <summary>
    /// OrganizationsContracts
    /// </summary>
    public OrganizationContractorModel[] OrganizationsContracts { get; set; } = default!;

    /// <summary>
    /// OrdersAttendances
    /// </summary>
    public OrderAnonModelDB[] OrdersAttendances { get; set; } = default!;

    /// <summary>
    /// Offers
    /// </summary>
    public OfferModelDB[] Offers { get; set; } = default!;

    /// <summary>
    /// WorkSchedulesViews
    /// </summary>
    public List<WorkSchedulesViewModel> WorksSchedulesViews()
    {
        List<WorkSchedulesViewModel> res = [];

        for (DateOnly dt = StartDate; dt <= EndDate; dt = dt.AddDays(1))
        {
            WeeklyScheduleModelDB[] w_sch = Schedules.Where(x => !x.IsDisabled && x.Weekday == dt.DayOfWeek).ToArray();
            CalendarScheduleModelDB[] c_sch = Calendars.Where(x => !x.IsDisabled && x.DateScheduleCalendar == dt).ToArray();

            if ((w_sch.Length == 0) && (c_sch.Length == 0))
                continue;

            if (c_sch.Length != 0)
            {
                foreach (CalendarScheduleModelDB csi in c_sch)
                {

                }
            }
            else if (w_sch.Length != 0)
            {
                foreach (WeeklyScheduleModelDB csi in w_sch)
                {

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