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
    public Task<TResponseModel<WorkSchedulesFindResponseModel>> WorkSchedulesFind(WorkSchedulesFindRequestModel req);

    /// <summary>
    /// WorkScheduleUpdate
    /// </summary>
    public Task<TResponseModel<int>> WorkScheduleUpdate(WorkScheduleModelDB work);

    /// <summary>
    /// WorkSchedulesSelect select
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<WorkScheduleModelDB>>> WorkSchedulesSelect(TPaginationRequestModel<WorkSchedulesSelectRequestModel> req);

    /// <summary>
    /// WorkSchedulesRead read
    /// </summary>
    public Task<TResponseModel<WorkScheduleModelDB[]>> WorkSchedulesRead(int[] req);

    /// <summary>
    /// WorkScheduleCalendarUpdate
    /// </summary>
    public Task<TResponseModel<int>> WorkScheduleCalendarUpdate(WorkScheduleCalendarModelDB work);

    /// <summary>
    /// WorkScheduleCalendarsSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<WorkScheduleCalendarModelDB>>> WorkScheduleCalendarsSelect(TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel> req);

    /// <summary>
    /// WorkScheduleCalendarsRead
    /// </summary>
    public Task<TResponseModel<WorkScheduleCalendarModelDB[]>> WorkScheduleCalendarsRead(int[] req);
}