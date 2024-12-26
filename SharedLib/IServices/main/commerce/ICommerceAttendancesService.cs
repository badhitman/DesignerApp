////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Attendance
/// </summary>
public partial interface ICommerceService
{
    public Task CreateAttendanceRecords(WorkScheduleModel[] workSchedules);

    /// <summary>
    /// WorkSchedulesSelect find
    /// </summary>
    public Task<WorkSchedulesFindResponseModel> WorkSchedulesFind(WorkSchedulesFindRequestModel req);

    /// <summary>
    /// WorkScheduleUpdate
    /// </summary>
    public Task<TResponseModel<int>> WeeklyScheduleUpdate(WeeklyScheduleModelDB work);

    /// <summary>
    /// WorkSchedulesSelect select
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<WeeklyScheduleModelDB>>> WeeklySchedulesSelect(TPaginationRequestModel<WorkSchedulesSelectRequestModel> req);

    /// <summary>
    /// WorkSchedulesRead read
    /// </summary>
    public Task<TResponseModel<WeeklyScheduleModelDB[]>> WeeklySchedulesRead(int[] req);

    /// <summary>
    /// WorkScheduleCalendarUpdate
    /// </summary>
    public Task<TResponseModel<int>> CalendarScheduleUpdate(CalendarScheduleModelDB work);

    /// <summary>
    /// WorkScheduleCalendarsSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<CalendarScheduleModelDB>>> CalendarSchedulesSelect(TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel> req);

    /// <summary>
    /// WorkScheduleCalendarsRead
    /// </summary>
    public Task<TResponseModel<CalendarScheduleModelDB[]>> CalendarSchedulesRead(int[] req);
}