////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

public partial interface ICommerceRemoteTransmissionService
{
    /// <summary>
    /// OrganizationOfferContractUpdate
    /// </summary>
    public Task<TResponseModel<bool>> OrganizationOfferContractUpdate(TAuthRequestModel<OrganizationOfferToggleModel> req);

    /// <summary>
    /// WorkSchedulesFind
    /// </summary>
    public Task<TResponseModel<WorkSchedulesFindResponseModel>> WorksSchedulesFind(WorkSchedulesFindRequestModel req);
    
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
    public Task<TResponseModel<TPaginationResponseModel<CalendarScheduleModelDB>>> CalendarsSchedulesSelect(TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel> req);

    /// <summary>
    /// WorkScheduleCalendarsRead
    /// </summary>
    public Task<TResponseModel<CalendarScheduleModelDB[]>> CalendarsSchedulesRead(int[] req);
}