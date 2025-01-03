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
    public async Task<bool> OrganizationOfferContractUpdate(TAuthRequestModel<OrganizationOfferToggleModel> req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.OrganizationOfferContractUpdateOrCreateCommerceReceive, req);

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
    public async Task<int> AddressOrganizationUpdate(AddressOrganizationBaseModel req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.AddressOrganizationUpdateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<int> NomenclatureUpdateReceive(NomenclatureModelDB req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.NomenclatureUpdateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<bool> OfferDelete(int req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.OfferDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<OfferModelDB>> OffersSelect(TPaginationRequestModel<OffersSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OfferModelDB>>(GlobalStaticConstants.TransmissionQueues.OfferSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<int> OfferUpdate(OfferModelDB offer)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.OfferUpdateCommerceReceive, offer);

    /// <inheritdoc/>
    public async Task<OrderDocumentModelDB[]> OrdersRead(int[] orders_ids)
        => await rabbitClient.MqRemoteCall<OrderDocumentModelDB[]>(GlobalStaticConstants.TransmissionQueues.OrdersReadCommerceReceive, orders_ids);

    /// <inheritdoc/>
    public async Task<bool> OrganizationSetLegal(OrganizationLegalModel org)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.OrganizationSetLegalCommerceReceive, org);

    /// <inheritdoc/>
    public async Task<OrganizationModelDB[]> OrganizationsRead(int[] req)
        => await rabbitClient.MqRemoteCall<OrganizationModelDB[]>(GlobalStaticConstants.TransmissionQueues.OrganizationsReadCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<OrganizationModelDB>> OrganizationsSelect(TPaginationRequestAuthModel<OrganizationsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OrganizationModelDB>>(GlobalStaticConstants.TransmissionQueues.OrganizationsSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<int> OrganizationUpdate(TAuthRequestModel<OrganizationModelDB> org)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.OrganizationUpdateOrCreateCommerceReceive, org);

    /// <inheritdoc/>
    public async Task<bool> PaymentDocumentDelete(int req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.PaymentDocumentDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<bool> AttachmentDeleteFromOrder(int req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.AttachmentDeleteFromOrderCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<int> RowForOrderUpdate(RowOfOrderDocumentModelDB row)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.RowForOrderUpdateCommerceReceive, row);

    /// <inheritdoc/>
    public async Task<bool> RowsForOrderDelete(int[] req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.RowsDeleteFromOrderCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<AddressOrganizationModelDB[]> AddressesOrganizationsRead(int[] ids)
        => await rabbitClient.MqRemoteCall<AddressOrganizationModelDB[]>(GlobalStaticConstants.TransmissionQueues.AddressesOrganizationsReadCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<NomenclatureModelDB[]> NomenclaturesRead(int[] ids)
        => await rabbitClient.MqRemoteCall<NomenclatureModelDB[]>(GlobalStaticConstants.TransmissionQueues.NomenclaturesReadCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<OfferModelDB[]> OffersRead(int[] ids)
        => await rabbitClient.MqRemoteCall<OfferModelDB[]>(GlobalStaticConstants.TransmissionQueues.OfferReadCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<PriceRuleForOfferModelDB[]> PricesRulesGetForOffers(int[] ids)
        => await rabbitClient.MqRemoteCall<PriceRuleForOfferModelDB[]>(GlobalStaticConstants.TransmissionQueues.PricesRulesGetForOfferCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<OrderDocumentModelDB>> OrdersSelect(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OrderDocumentModelDB>>(GlobalStaticConstants.TransmissionQueues.OrdersSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<int> OrderUpdate(OrderDocumentModelDB order)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.OrderUpdateCommerceReceive, order);

    /// <inheritdoc/>
    public async Task<int> PaymentDocumentUpdate(PaymentDocumentBaseModel payment)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.PaymentDocumentUpdateCommerceReceive, payment);

    /// <inheritdoc/>
    public async Task<int> PriceRuleUpdate(PriceRuleForOfferModelDB price_rule)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.PriceRuleUpdateCommerceReceive, price_rule);

    /// <inheritdoc/>
    public async Task<bool> PriceRuleDelete(int id)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.PriceRuleDeleteCommerceReceive, id);

    /// <inheritdoc/>
    public async Task<bool> StatusOrderChange(StatusChangeRequestModel req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.StatusChangeOrderByHelpDeskDocumentIdReceive, req, waitResponse);

    /// <inheritdoc/>
    public async Task<OrderDocumentModelDB[]> OrdersByIssues(OrdersByIssuesSelectRequestModel req)
        => await rabbitClient.MqRemoteCall<OrderDocumentModelDB[]>(GlobalStaticConstants.TransmissionQueues.OrdersByIssuesGetReceive, req);

    /// <inheritdoc/>
    public async Task<FileAttachModel> OrderReportGet(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<FileAttachModel>(GlobalStaticConstants.TransmissionQueues.OrderReportGetCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<FileAttachModel> PriceFullFileGet()
        => await rabbitClient.MqRemoteCall<FileAttachModel>(GlobalStaticConstants.TransmissionQueues.PriceFullFileGetCommerceReceive);

    /// <inheritdoc/>
    public async Task<int> WeeklyScheduleUpdate(WeeklyScheduleModelDB work)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.WeeklyScheduleUpdateCommerceReceive, work);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<WeeklyScheduleModelDB>> WeeklySchedulesSelect(TPaginationRequestModel<WorkSchedulesSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<WeeklyScheduleModelDB>>(GlobalStaticConstants.TransmissionQueues.WeeklySchedulesSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<WeeklyScheduleModelDB[]> WeeklySchedulesRead(int[] req)
        => await rabbitClient.MqRemoteCall<WeeklyScheduleModelDB[]>(GlobalStaticConstants.TransmissionQueues.WeeklySchedulesReadCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<int> CalendarScheduleUpdate(CalendarScheduleModelDB work)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.CalendarScheduleUpdateCommerceReceive, work);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<CalendarScheduleModelDB>> CalendarsSchedulesSelect(TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<CalendarScheduleModelDB>>(GlobalStaticConstants.TransmissionQueues.CalendarsSchedulesSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<CalendarScheduleModelDB[]> CalendarsSchedulesRead(int[] req)
        => await rabbitClient.MqRemoteCall<CalendarScheduleModelDB[]>(GlobalStaticConstants.TransmissionQueues.CalendarsSchedulesReadCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<UserOrganizationModelDB>> UsersOrganizationsSelect(TPaginationRequestAuthModel<UsersOrganizationsStatusesRequest> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<UserOrganizationModelDB>>(GlobalStaticConstants.TransmissionQueues.OrganizationsUsersSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<int> UserOrganizationUpdate(TAuthRequestModel<UserOrganizationModelDB> org)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.OrganizationUserUpdateOrCreateCommerceReceive, org);

    /// <inheritdoc/>
    public async Task<UserOrganizationModelDB[]> UsersOrganizationsRead(int[] organizations_ids)
        => await rabbitClient.MqRemoteCall<UserOrganizationModelDB[]>(GlobalStaticConstants.TransmissionQueues.OrganizationsUsersReadCommerceReceive, organizations_ids);

    /// <inheritdoc/>
    public async Task<OrderAttendanceModelDB[]> OrdersAttendancesByIssues(OrdersByIssuesSelectRequestModel req)
        => await rabbitClient.MqRemoteCall<OrderAttendanceModelDB[]>(GlobalStaticConstants.TransmissionQueues.OrdersAttendancesByIssuesGetReceive, req);

    /// <inheritdoc/>
    public async Task<bool> StatusesOrdersAttendancesChangeByHelpdeskDocumentId(TAuthRequestModel<StatusChangeRequestModel> req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.OrdersAttendancesStatusesChangeByHelpdeskDocumentIdReceive, req);
}