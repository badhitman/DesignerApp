////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// CommerceTransmission
/// </summary>
public partial class CommerceTransmission(IRabbitClient rabbitClient) : ICommerceTransmission
{
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CreateAttendanceRecords(TAuthRequestModel<CreateAttendanceRequestModel> req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.CreateAttendanceRecordsCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> StatusesOrdersAttendancesChangeByHelpdeskDocumentId(TAuthRequestModel<StatusChangeRequestModel> req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.OrdersAttendancesStatusesChangeByHelpdeskDocumentIdReceive, req, waitResponse) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> OrganizationOfferContractUpdate(TAuthRequestModel<OrganizationOfferToggleModel> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.OrganizationOfferContractUpdateOrCreateCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<WorksFindResponseModel> WorksSchedulesFind(WorkFindRequestModel req)
        => await rabbitClient.MqRemoteCall<WorksFindResponseModel>(GlobalStaticConstants.TransmissionQueues.WorksSchedulesFindCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<NomenclatureModelDB>> NomenclaturesSelect(TPaginationRequestModel<NomenclaturesSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<NomenclatureModelDB>>(GlobalStaticConstants.TransmissionQueues.NomenclaturesSelectCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddressOrganizationDelete(int req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.AddressOrganizationDeleteCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> AddressOrganizationUpdate(AddressOrganizationBaseModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.AddressOrganizationUpdateCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> NomenclatureUpdateReceive(NomenclatureModelDB req)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.NomenclatureUpdateCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> OfferDelete(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.OfferDeleteCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OfferModelDB>>> OffersSelect(TAuthRequestModel<TPaginationRequestModel<OffersSelectRequestModel>> req)
        => await rabbitClient.MqRemoteCall< TResponseModel<TPaginationResponseModel<OfferModelDB>>>(GlobalStaticConstants.TransmissionQueues.OfferSelectCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OfferUpdate(TAuthRequestModel<OfferModelDB> offer)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.OfferUpdateCommerceReceive, offer) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>> OrdersRead(TAuthRequestModel<int[]> orders_ids)
        => await rabbitClient.MqRemoteCall<TResponseModel<OrderDocumentModelDB[]>>(GlobalStaticConstants.TransmissionQueues.OrdersReadCommerceReceive, orders_ids) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> OrganizationSetLegal(OrganizationLegalModel org)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.OrganizationSetLegalCommerceReceive, org) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<OrganizationModelDB[]>> OrganizationsRead(int[] req)
        => await rabbitClient.MqRemoteCall<TResponseModel<OrganizationModelDB[]>>(GlobalStaticConstants.TransmissionQueues.OrganizationsReadCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<OrganizationModelDB>> OrganizationsSelect(TPaginationRequestAuthModel<OrganizationsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OrganizationModelDB>>(GlobalStaticConstants.TransmissionQueues.OrganizationsSelectCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OrganizationUpdate(TAuthRequestModel<OrganizationModelDB> org)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.OrganizationUpdateOrCreateCommerceReceive, org) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> PaymentDocumentDelete(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.PaymentDocumentDeleteCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> RowForOrderUpdate(RowOfOrderDocumentModelDB row)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.RowForOrderUpdateCommerceReceive, row) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RowsForOrderDelete(int[] req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.RowsDeleteFromOrderCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<AddressOrganizationModelDB[]>> AddressesOrganizationsRead(int[] ids)
        => await rabbitClient.MqRemoteCall<TResponseModel<AddressOrganizationModelDB[]>>(GlobalStaticConstants.TransmissionQueues.AddressesOrganizationsReadCommerceReceive, ids) ?? new();

    /// <inheritdoc/> int[]?, List<NomenclatureModelDB>?
    public async Task<TResponseModel<List<NomenclatureModelDB>>> NomenclaturesRead(TAuthRequestModel<int[]> ids)
        => await rabbitClient.MqRemoteCall<TResponseModel<List<NomenclatureModelDB>>>(GlobalStaticConstants.TransmissionQueues.NomenclaturesReadCommerceReceive, ids) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<OfferModelDB[]>> OffersRead(TAuthRequestModel<int[]> ids)
        => await rabbitClient.MqRemoteCall<TResponseModel<OfferModelDB[]>>(GlobalStaticConstants.TransmissionQueues.OfferReadCommerceReceive, ids) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<List<PriceRuleForOfferModelDB>>> PricesRulesGetForOffers(TAuthRequestModel<int[]> ids)
        => await rabbitClient.MqRemoteCall<TResponseModel<List<PriceRuleForOfferModelDB>>>(GlobalStaticConstants.TransmissionQueues.PricesRulesGetForOfferCommerceReceive, ids) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<OrderDocumentModelDB>> OrdersSelect(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OrderDocumentModelDB>>(GlobalStaticConstants.TransmissionQueues.OrdersSelectCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OrderUpdate(OrderDocumentModelDB order)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.OrderUpdateCommerceReceive, order) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> PaymentDocumentUpdate(TAuthRequestModel<PaymentDocumentBaseModel> payment)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.PaymentDocumentUpdateCommerceReceive, payment) ?? new();

    /// <inheritdoc/> PriceRuleForOfferModelDB?, TResponseModel<int>?
    public async Task<TResponseModel<int>> PriceRuleUpdate(TAuthRequestModel<PriceRuleForOfferModelDB> price_rule)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.PriceRuleUpdateCommerceReceive, price_rule) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> PriceRuleDelete(TAuthRequestModel<int> id)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.PriceRuleDeleteCommerceReceive, id) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> StatusOrderChangeByHelpdeskDocumentId(TAuthRequestModel<StatusChangeRequestModel> req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.StatusChangeOrderByHelpDeskDocumentIdReceive, req, waitResponse) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>> OrdersByIssues(OrdersByIssuesSelectRequestModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<OrderDocumentModelDB[]>>(GlobalStaticConstants.TransmissionQueues.OrdersByIssuesGetReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<FileAttachModel>> OrderReportGet(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<FileAttachModel>>(GlobalStaticConstants.TransmissionQueues.OrderReportGetCommerceReceive, req) ?? new();

    /// <inheritdoc/> object?, FileAttachModel?
    public async Task<FileAttachModel> PriceFullFileGet()
        => await rabbitClient.MqRemoteCall<FileAttachModel>(GlobalStaticConstants.TransmissionQueues.PriceFullFileGetCommerceReceive) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> WeeklyScheduleUpdate(WeeklyScheduleModelDB work)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.WeeklyScheduleUpdateCommerceReceive, work) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<WeeklyScheduleModelDB>> WeeklySchedulesSelect(TPaginationRequestModel<WorkSchedulesSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<WeeklyScheduleModelDB>>(GlobalStaticConstants.TransmissionQueues.WeeklySchedulesSelectCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<List<WeeklyScheduleModelDB>> WeeklySchedulesRead(int[] req)
        => await rabbitClient.MqRemoteCall<List<WeeklyScheduleModelDB>>(GlobalStaticConstants.TransmissionQueues.WeeklySchedulesReadCommerceReceive, req) ?? [];

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CalendarScheduleUpdate(TAuthRequestModel<CalendarScheduleModelDB> work)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.CalendarScheduleUpdateCommerceReceive, work) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<CalendarScheduleModelDB>>> CalendarsSchedulesSelect(TAuthRequestModel<TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel>> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<TPaginationResponseModel<CalendarScheduleModelDB>>>(GlobalStaticConstants.TransmissionQueues.CalendarsSchedulesSelectCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<List<CalendarScheduleModelDB>>> CalendarsSchedulesRead(TAuthRequestModel<int[]> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<List<CalendarScheduleModelDB>>>(GlobalStaticConstants.TransmissionQueues.CalendarsSchedulesReadCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<UserOrganizationModelDB>>> UsersOrganizationsSelect(TPaginationRequestAuthModel<UsersOrganizationsStatusesRequest> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<TPaginationResponseModel<UserOrganizationModelDB>>>(GlobalStaticConstants.TransmissionQueues.OrganizationsUsersSelectCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> UserOrganizationUpdate(TAuthRequestModel<UserOrganizationModelDB> org)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.OrganizationUserUpdateOrCreateCommerceReceive, org) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<UserOrganizationModelDB[]>> UsersOrganizationsRead(int[] organizations_ids)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserOrganizationModelDB[]>>(GlobalStaticConstants.TransmissionQueues.OrganizationsUsersReadCommerceReceive, organizations_ids) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<RecordsAttendanceModelDB[]>> OrdersAttendancesByIssues(OrdersByIssuesSelectRequestModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<RecordsAttendanceModelDB[]>>(GlobalStaticConstants.TransmissionQueues.OrdersAttendancesByIssuesGetReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AttendanceRecordsDelete(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.AttendanceRecordDeleteCommerceReceive, req) ?? new();
}