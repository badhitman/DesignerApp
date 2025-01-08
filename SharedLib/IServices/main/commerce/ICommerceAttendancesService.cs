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
    /// Удалить бронь
    /// </summary>
    public Task<ResponseBaseModel> OrderAttendanceDeleteRecord(TAuthRequestModel<int> orderId);

    /// <summary>
    /// Смена статуса заказу по идентификатору HelpDesk документа
    /// </summary>
    /// <remarks>
    /// В запросе нельзя указывать идентификатор заказа: только идентификатор HelpDesk документа.
    /// Допускается ситуация, когда под одним идентификатором HelpDesk документа могут существовать несколько заказов (объединённые заказы).
    /// </remarks>
    public Task<TResponseModel<bool>> StatusesOrdersAttendancesChangeByHelpdeskDocumentId(TAuthRequestModel<StatusChangeRequestModel> req);

    /// <summary>
    /// OrdersByIssuesGet
    /// </summary>
    public Task<TResponseModel<OrderAttendanceModelDB[]>> AttendancesRecordsByIssuesGet(OrdersByIssuesSelectRequestModel req);

    /// <summary>
    /// Create attendance records
    /// </summary>
    public Task<ResponseBaseModel> CreateAttendanceRecords(TAuthRequestModel<CreateAttendanceRequestModel> workSchedules);

    /// <summary>
    /// WorkSchedulesSelect find
    /// </summary>
    public Task<WorksFindResponseModel> WorkSchedulesFind(WorkFindRequestModel req, int[]? organizationsFilter = null);

    /// <summary>
    /// WorkScheduleUpdate
    /// </summary>
    public Task<TResponseModel<int>> WeeklyScheduleUpdate(WeeklyScheduleModelDB work);

    /// <summary>
    /// WorkSchedulesSelect select
    /// </summary>
    public Task<TPaginationResponseModel<WeeklyScheduleModelDB>> WeeklySchedulesSelect(TPaginationRequestModel<WorkSchedulesSelectRequestModel> req);

    /// <summary>
    /// WorkSchedulesRead read
    /// </summary>
    public Task<List<WeeklyScheduleModelDB>> WeeklySchedulesRead(int[] req);

    /// <summary>
    /// WorkScheduleCalendarUpdate
    /// </summary>
    public Task<TResponseModel<int>> CalendarScheduleUpdate(TAuthRequestModel<CalendarScheduleModelDB> work);

    /// <summary>
    /// WorkScheduleCalendarsSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<CalendarScheduleModelDB>>> CalendarSchedulesSelect(TAuthRequestModel<TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel>> req);

    /// <summary>
    /// WorkScheduleCalendarsRead
    /// </summary>
    public Task<TResponseModel<List<CalendarScheduleModelDB>>> CalendarSchedulesRead(TAuthRequestModel<int[]> req);
}