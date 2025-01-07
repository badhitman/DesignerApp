////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

public partial interface ICommerceRemoteTransmissionService
{
    /// <summary>
    /// Смена статуса заявки (бронь)
    /// </summary>
    public Task<bool> StatusesOrdersAttendancesChangeByHelpdeskDocumentId(TAuthRequestModel<StatusChangeRequestModel> req);

    /// <summary>
    /// Получить заказы (по заявкам)
    /// </summary>
    public Task<TResponseModel<OrderAttendanceModelDB[]>> OrdersAttendancesByIssues(OrdersByIssuesSelectRequestModel req);

    /// <summary>
    /// Create attendance records
    /// </summary>
    public Task<ResponseBaseModel> CreateAttendanceRecords(TAuthRequestModel<CreateAttendanceRequestModel> workSchedules);

    /// <summary>
    /// OrganizationOfferContractUpdate
    /// </summary>
    public Task<TResponseModel<bool>> OrganizationOfferContractUpdate(TAuthRequestModel<OrganizationOfferToggleModel> req);

    /// <summary>
    /// WorkSchedulesFind
    /// </summary>
    public Task<WorkSchedulesFindResponseModel> WorksSchedulesFind(WorkSchedulesFindRequestModel req);

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
    public Task<TResponseModel<TPaginationResponseModel<CalendarScheduleModelDB>>> CalendarsSchedulesSelect(TAuthRequestModel<TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel>> req);

    /// <summary>
    /// WorkScheduleCalendarsRead
    /// </summary>
    public Task<TResponseModel<List<CalendarScheduleModelDB>>> CalendarsSchedulesRead(TAuthRequestModel<int[]> req);
}