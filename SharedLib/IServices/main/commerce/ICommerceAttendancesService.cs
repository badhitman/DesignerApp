////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Attendance
/// </summary>
public partial interface ICommerceService
{
    /// <summary>
    /// WorkSchedulesSelect find
    /// </summary>
    public Task<WorkSchedulesFindResponseModel> WorkSchedulesFind(WorkSchedulesFindRequestModel req);

    /// <summary>
    /// WorkScheduleUpdate
    /// </summary>
    public Task<TResponseModel<int>> WorkScheduleUpdate(WeeklyScheduleModelDB work);

    /// <summary>
    /// WorkSchedulesSelect select
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<WeeklyScheduleModelDB>>> WorkSchedulesSelect(TPaginationRequestModel<WorkSchedulesSelectRequestModel> req);

    /// <summary>
    /// WorkSchedulesRead read
    /// </summary>
    public Task<TResponseModel<WeeklyScheduleModelDB[]>> WorkSchedulesRead(int[] req);

    /// <summary>
    /// WorkScheduleCalendarUpdate
    /// </summary>
    public Task<TResponseModel<int>> WorkScheduleCalendarUpdate(CalendarScheduleModelDB work);

    /// <summary>
    /// WorkScheduleCalendarsSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<CalendarScheduleModelDB>>> WorkScheduleCalendarsSelect(TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel> req);

    /// <summary>
    /// WorkScheduleCalendarsRead
    /// </summary>
    public Task<TResponseModel<CalendarScheduleModelDB[]>> WorkScheduleCalendarsRead(int[] req);
}