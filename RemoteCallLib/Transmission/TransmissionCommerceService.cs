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
    public async Task<TResponseModel<bool>> OrganizationOfferContractUpdate(TAuthRequestModel<OrganizationOfferToggleModel> req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.OrganizationOfferContractUpdateOrCreateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<WorkSchedulesFindResponseModel>> WorkSchedulesFind(WorkSchedulesFindRequestModel req)
        => await rabbitClient.MqRemoteCall<WorkSchedulesFindResponseModel>(GlobalStaticConstants.TransmissionQueues.WorksSchedulesFindCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<NomenclatureModelDB>>> NomenclaturesSelect(TPaginationRequestModel<NomenclaturesSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<NomenclatureModelDB>>(GlobalStaticConstants.TransmissionQueues.NomenclaturesSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddressOrganizationDelete(int req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.AddressOrganizationDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> AddressOrganizationUpdate(AddressOrganizationBaseModel req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.AddressOrganizationUpdateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> NomenclatureUpdateReceive(NomenclatureModelDB req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.NomenclatureUpdateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> OfferDelete(int req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.OfferDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OfferModelDB>>> OffersSelect(TPaginationRequestModel<OffersSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OfferModelDB>>(GlobalStaticConstants.TransmissionQueues.OfferSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OfferUpdate(OfferModelDB offer)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.OfferUpdateCommerceReceive, offer);

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>> OrdersRead(int[] orders_ids)
        => await rabbitClient.MqRemoteCall<OrderDocumentModelDB[]>(GlobalStaticConstants.TransmissionQueues.OrdersReadCommerceReceive, orders_ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> OrganizationSetLegal(OrganizationLegalModel org)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.OrganizationSetLegalCommerceReceive, org);

    /// <inheritdoc/>
    public async Task<TResponseModel<OrganizationModelDB[]>> OrganizationsRead(int[] req)
        => await rabbitClient.MqRemoteCall<OrganizationModelDB[]>(GlobalStaticConstants.TransmissionQueues.OrganizationsReadCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>>> OrganizationsSelect(TPaginationRequestAuthModel<OrganizationsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OrganizationModelDB>>(GlobalStaticConstants.TransmissionQueues.OrganizationsSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OrganizationUpdate(TAuthRequestModel<OrganizationModelDB> org)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.OrganizationUpdateOrCreateCommerceReceive, org);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> PaymentDocumentDelete(int req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.PaymentDocumentDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> AttachmentDeleteFromOrder(int req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.AttachmentDeleteFromOrderCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> RowForOrderUpdate(RowOfOrderDocumentModelDB row)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.RowForOrderUpdateCommerceReceive, row);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RowsForOrderDelete(int[] req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.RowsDeleteFromOrderCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<AddressOrganizationModelDB[]>> AddressesOrganizationsRead(int[] ids)
        => await rabbitClient.MqRemoteCall<AddressOrganizationModelDB[]>(GlobalStaticConstants.TransmissionQueues.AddressesOrganizationsReadCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<NomenclatureModelDB[]>> NomenclaturesRead(int[] ids)
        => await rabbitClient.MqRemoteCall<NomenclatureModelDB[]>(GlobalStaticConstants.TransmissionQueues.NomenclaturesReadCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<OfferModelDB[]>> OffersRead(int[] ids)
        => await rabbitClient.MqRemoteCall<OfferModelDB[]>(GlobalStaticConstants.TransmissionQueues.OfferReadCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<PriceRuleForOfferModelDB[]>> PricesRulesGetForOffers(int[] ids)
        => await rabbitClient.MqRemoteCall<PriceRuleForOfferModelDB[]>(GlobalStaticConstants.TransmissionQueues.PricesRulesGetForOfferCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>> OrdersSelect(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OrderDocumentModelDB>>(GlobalStaticConstants.TransmissionQueues.OrdersSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OrderUpdate(OrderDocumentModelDB order)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.OrderUpdateCommerceReceive, order);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> PaymentDocumentUpdate(PaymentDocumentBaseModel payment)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.PaymentDocumentUpdateCommerceReceive, payment);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> PriceRuleUpdate(PriceRuleForOfferModelDB price_rule)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.PriceRuleUpdateCommerceReceive, price_rule);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> PriceRuleDelete(int id)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.PriceRuleDeleteCommerceReceive, id);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> StatusOrderChange(StatusOrderChangeRequestModel req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.StatusChangeOrderByHelpDeskDocumentIdReceive, req, waitResponse);

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>> OrdersByIssues(OrdersByIssuesSelectRequestModel req)
        => await rabbitClient.MqRemoteCall<OrderDocumentModelDB[]>(GlobalStaticConstants.TransmissionQueues.OrdersByIssuesGetReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<FileAttachModel>> OrderReportGet(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<FileAttachModel>(GlobalStaticConstants.TransmissionQueues.OrderReportGetCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<FileAttachModel>> PriceFullFileGet()
        => await rabbitClient.MqRemoteCall<FileAttachModel>(GlobalStaticConstants.TransmissionQueues.PriceFullFileGetCommerceReceive);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> WorkScheduleUpdate(WeeklyScheduleModelDB work)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.WorkScheduleUpdateCommerceReceive, work);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<WeeklyScheduleModelDB>>> WorkSchedulesSelect(TPaginationRequestModel<WorkSchedulesSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<WeeklyScheduleModelDB>>(GlobalStaticConstants.TransmissionQueues.WorkSchedulesSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<WeeklyScheduleModelDB[]>> WorkSchedulesRead(int[] req)
        => await rabbitClient.MqRemoteCall<WeeklyScheduleModelDB[]>(GlobalStaticConstants.TransmissionQueues.WorkSchedulesReadCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> WorkScheduleCalendarUpdate(WorkScheduleCalendarModelDB work)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.WorkScheduleCalendarUpdateCommerceReceive, work);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<WorkScheduleCalendarModelDB>>> WorkScheduleCalendarsSelect(TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<WorkScheduleCalendarModelDB>>(GlobalStaticConstants.TransmissionQueues.WorkScheduleCalendarsSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<WorkScheduleCalendarModelDB[]>> WorkScheduleCalendarsRead(int[] req)
        => await rabbitClient.MqRemoteCall<WorkScheduleCalendarModelDB[]>(GlobalStaticConstants.TransmissionQueues.WorkScheduleCalendarsReadCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<UserOrganizationModelDB>>> UsersOrganizationsSelect(TPaginationRequestAuthModel<UsersOrganizationsStatusesRequest> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<UserOrganizationModelDB>>(GlobalStaticConstants.TransmissionQueues.OrganizationsUsersSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> UserOrganizationUpdate(TAuthRequestModel<UserOrganizationModelDB> org)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.OrganizationUserUpdateOrCreateCommerceReceive, org);

    /// <inheritdoc/>
    public async Task<TResponseModel<UserOrganizationModelDB[]>> UsersOrganizationsRead(int[] organizations_ids)
        => await rabbitClient.MqRemoteCall<UserOrganizationModelDB[]>(GlobalStaticConstants.TransmissionQueues.OrganizationsUsersReadCommerceReceive, organizations_ids);
}