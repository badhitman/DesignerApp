////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Бронь/Запись (услуги/аренда)
/// </summary>
public partial interface ICommerceService
{
    #region records
    /// <summary>
    /// Подбор записей (актуальных)
    /// </summary>
    public Task<TPaginationResponseModel<RecordsAttendanceModelDB>> RecordsAttendancesSelect(TPaginationRequestAuthModel<RecordsAttendancesRequestModel> req);

    /// <summary>
    /// Удалить запись/бронь
    /// </summary>
    public Task<ResponseBaseModel> RecordAttendanceDelete(TAuthRequestModel<int> orderId);

    /// <summary>
    /// Смена статуса записи/брони по идентификатору HelpDesk документа
    /// </summary>
    /// <remarks>
    /// В запросе нельзя указывать идентификатор заказа: только идентификатор HelpDesk документа.
    /// Допускается ситуация, когда под одним идентификатором HelpDesk документа могут существовать несколько заказов (объединённые заказы).
    /// </remarks>
    public Task<TResponseModel<bool>> RecordsAttendancesStatusesChangeByHelpdeskId(TAuthRequestModel<StatusChangeRequestModel> req);

    /// <summary>
    /// Получить брони/записи по HelpDesk
    /// </summary>
    public Task<TResponseModel<RecordsAttendanceModelDB[]>> RecordsAttendancesByIssuesGet(OrdersByIssuesSelectRequestModel req);

    /// <summary>
    /// Создать пакет записей/броней
    /// </summary>
    public Task<ResponseBaseModel> RecordsAttendanceCreate(TAuthRequestModel<CreateAttendanceRequestModel> workSchedules);
    #endregion

    /// <summary>
    /// Поиск доступных услуг/броней
    /// </summary>
    public Task<WorksFindResponseModel> WorkSchedulesFind(WorkFindRequestModel req, int[]? organizationsFilter = null);

    /// <summary>
    /// Недельное расписание обновить/создать
    /// </summary>
    public Task<TResponseModel<int>> WeeklyScheduleUpdate(WeeklyScheduleModelDB work);

    /// <summary>
    /// Подбор недельных расписаний
    /// </summary>
    public Task<TPaginationResponseModel<WeeklyScheduleModelDB>> WeeklySchedulesSelect(TPaginationRequestModel<WorkSchedulesSelectRequestModel> req);

    /// <summary>
    /// Прочитать недельные расписания (по идентификаторам)
    /// </summary>
    public Task<List<WeeklyScheduleModelDB>> WeeklySchedulesRead(int[] req);

    /// <summary>
    /// Расписание на дату создать/обновить
    /// </summary>
    public Task<TResponseModel<int>> CalendarScheduleUpdate(TAuthRequestModel<CalendarScheduleModelDB> work);

    /// <summary>
    /// Подбор расписаний для дат
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<CalendarScheduleModelDB>>> CalendarSchedulesSelect(TAuthRequestModel<TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel>> req);

    /// <summary>
    /// Расписания для дат прочитать по их иддентификаторам
    /// </summary>
    public Task<TResponseModel<List<CalendarScheduleModelDB>>> CalendarSchedulesRead(TAuthRequestModel<int[]> req);
}