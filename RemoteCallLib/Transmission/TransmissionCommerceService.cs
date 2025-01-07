////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// TransmissionCommerceService
/// </summary>
public partial class TransmissionCommerceService(IRabbitClient rabbitClient) : ICommerceRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CreateAttendanceRecords(TAuthRequestModel<CreateAttendanceRequestModel> req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.CreateAttendanceRecordsCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> OrganizationOfferContractUpdate(TAuthRequestModel<OrganizationOfferToggleModel> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.OrganizationOfferContractUpdateOrCreateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<WorkSchedulesFindResponseModel> WorksSchedulesFind(WorkSchedulesFindRequestModel req)
        => await rabbitClient.MqRemoteCall<WorkSchedulesFindResponseModel>(GlobalStaticConstants.TransmissionQueues.WorksSchedulesFindCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<NomenclatureModelDB>> NomenclaturesSelect(TPaginationRequestModel<NomenclaturesSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<NomenclatureModelDB>>(GlobalStaticConstants.TransmissionQueues.NomenclaturesSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddressOrganizationDelete(int req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.AddressOrganizationDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> AddressOrganizationUpdate(AddressOrganizationBaseModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.AddressOrganizationUpdateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> NomenclatureUpdateReceive(NomenclatureModelDB req)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.NomenclatureUpdateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> OfferDelete(int req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.OfferDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<OfferModelDB>> OffersSelect(TPaginationRequestModel<OffersSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OfferModelDB>>(GlobalStaticConstants.TransmissionQueues.OfferSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OfferUpdate(OfferModelDB offer)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.OfferUpdateCommerceReceive, offer);

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>> OrdersRead(int[] orders_ids)
        => await rabbitClient.MqRemoteCall<TResponseModel<OrderDocumentModelDB[]>>(GlobalStaticConstants.TransmissionQueues.OrdersReadCommerceReceive, orders_ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> OrganizationSetLegal(OrganizationLegalModel org)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.OrganizationSetLegalCommerceReceive, org);

    /// <inheritdoc/>
    public async Task<TResponseModel<OrganizationModelDB[]>> OrganizationsRead(int[] req)
        => await rabbitClient.MqRemoteCall<TResponseModel<OrganizationModelDB[]>>(GlobalStaticConstants.TransmissionQueues.OrganizationsReadCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<OrganizationModelDB>> OrganizationsSelect(TPaginationRequestAuthModel<OrganizationsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OrganizationModelDB>>(GlobalStaticConstants.TransmissionQueues.OrganizationsSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OrganizationUpdate(TAuthRequestModel<OrganizationModelDB> org)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.OrganizationUpdateOrCreateCommerceReceive, org);

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> PaymentDocumentDelete(int req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.PaymentDocumentDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> RowForOrderUpdate(RowOfOrderDocumentModelDB row)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.RowForOrderUpdateCommerceReceive, row);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RowsForOrderDelete(int[] req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.RowsDeleteFromOrderCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<AddressOrganizationModelDB[]>> AddressesOrganizationsRead(int[] ids)
        => await rabbitClient.MqRemoteCall<TResponseModel<AddressOrganizationModelDB[]>>(GlobalStaticConstants.TransmissionQueues.AddressesOrganizationsReadCommerceReceive, ids);

    /// <inheritdoc/> int[]?, List<NomenclatureModelDB>?
    public async Task<List<NomenclatureModelDB>> NomenclaturesRead(int[] ids)
        => await rabbitClient.MqRemoteCall<List<NomenclatureModelDB>>(GlobalStaticConstants.TransmissionQueues.NomenclaturesReadCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<OfferModelDB[]>> OffersRead(int[] ids)
        => await rabbitClient.MqRemoteCall<TResponseModel<OfferModelDB[]>>(GlobalStaticConstants.TransmissionQueues.OfferReadCommerceReceive, ids);

    /// <inheritdoc/> int[]?, List<PriceRuleForOfferModelDB>?
    public async Task<List<PriceRuleForOfferModelDB>> PricesRulesGetForOffers(int[] ids)
        => await rabbitClient.MqRemoteCall<List<PriceRuleForOfferModelDB>>(GlobalStaticConstants.TransmissionQueues.PricesRulesGetForOfferCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>> OrdersSelect(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>>(GlobalStaticConstants.TransmissionQueues.OrdersSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OrderUpdate(OrderDocumentModelDB order)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.OrderUpdateCommerceReceive, order);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> PaymentDocumentUpdate(PaymentDocumentBaseModel payment)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.PaymentDocumentUpdateCommerceReceive, payment);

    /// <inheritdoc/> PriceRuleForOfferModelDB?, TResponseModel<int>?
    public async Task<TResponseModel<int>> PriceRuleUpdate(PriceRuleForOfferModelDB price_rule)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.PriceRuleUpdateCommerceReceive, price_rule);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> PriceRuleDelete(int id)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.PriceRuleDeleteCommerceReceive, id);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> StatusOrderChange(StatusChangeRequestModel req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.StatusChangeOrderByHelpDeskDocumentIdReceive, req, waitResponse);

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>> OrdersByIssues(OrdersByIssuesSelectRequestModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<OrderDocumentModelDB[]>>(GlobalStaticConstants.TransmissionQueues.OrdersByIssuesGetReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<FileAttachModel>> OrderReportGet(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<FileAttachModel>>(GlobalStaticConstants.TransmissionQueues.OrderReportGetCommerceReceive, req);

    /// <inheritdoc/> object?, FileAttachModel?
    public async Task<FileAttachModel> PriceFullFileGet()
        => await rabbitClient.MqRemoteCall<FileAttachModel>(GlobalStaticConstants.TransmissionQueues.PriceFullFileGetCommerceReceive);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> WeeklyScheduleUpdate(WeeklyScheduleModelDB work)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.WeeklyScheduleUpdateCommerceReceive, work);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<WeeklyScheduleModelDB>> WeeklySchedulesSelect(TPaginationRequestModel<WorkSchedulesSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<WeeklyScheduleModelDB>>(GlobalStaticConstants.TransmissionQueues.WeeklySchedulesSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<List<WeeklyScheduleModelDB>> WeeklySchedulesRead(int[] req)
        => await rabbitClient.MqRemoteCall<List<WeeklyScheduleModelDB>>(GlobalStaticConstants.TransmissionQueues.WeeklySchedulesReadCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CalendarScheduleUpdate(CalendarScheduleModelDB work)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.CalendarScheduleUpdateCommerceReceive, work);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<CalendarScheduleModelDB>> CalendarsSchedulesSelect(TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<CalendarScheduleModelDB>>(GlobalStaticConstants.TransmissionQueues.CalendarsSchedulesSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<List<CalendarScheduleModelDB>> CalendarsSchedulesRead(int[] req)
        => await rabbitClient.MqRemoteCall<List<CalendarScheduleModelDB>>(GlobalStaticConstants.TransmissionQueues.CalendarsSchedulesReadCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<UserOrganizationModelDB>>> UsersOrganizationsSelect(TPaginationRequestAuthModel<UsersOrganizationsStatusesRequest> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<TPaginationResponseModel<UserOrganizationModelDB>>>(GlobalStaticConstants.TransmissionQueues.OrganizationsUsersSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> UserOrganizationUpdate(TAuthRequestModel<UserOrganizationModelDB> org)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.OrganizationUserUpdateOrCreateCommerceReceive, org);

    /// <inheritdoc/>
    public async Task<TResponseModel<UserOrganizationModelDB[]>> UsersOrganizationsRead(int[] organizations_ids)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserOrganizationModelDB[]>>(GlobalStaticConstants.TransmissionQueues.OrganizationsUsersReadCommerceReceive, organizations_ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderAttendanceModelDB[]>> OrdersAttendancesByIssues(OrdersByIssuesSelectRequestModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<OrderAttendanceModelDB[]>>(GlobalStaticConstants.TransmissionQueues.OrdersAttendancesByIssuesGetReceive, req);

    /// <inheritdoc/>
    public async Task<bool> StatusesOrdersAttendancesChangeByHelpdeskDocumentId(TAuthRequestModel<StatusChangeRequestModel> req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.StatusChangeOrderByHelpDeskDocumentIdReceive, req);
}