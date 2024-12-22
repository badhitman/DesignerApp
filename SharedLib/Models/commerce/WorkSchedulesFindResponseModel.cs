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
    /// WorkSchedulesViews
    /// </summary>
    public List<WorkSchedulesViewModel> WorkSchedulesViews()
    {
        List<WorkSchedulesViewModel> res = [];
        //WorkSchedulesViewModel _el;
        //for (DateOnly dt = StartDate; dt <= EndDate; dt = dt.AddDays(1))
        //{
        //    _el = new()
        //    {
        //        Date = dt,                 
        //    };

        //    res.Add(_el);
        //}

        return res;
    }
}