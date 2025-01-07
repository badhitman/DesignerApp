////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Transmission.Receives.commerce;
using SharedLib;

namespace CommerceService;

/// <summary>
/// MQ listen
/// </summary>
public static class RegisterMqListenerExtension
{
    /// <summary>
    /// RegisterMqListeners
    /// </summary>
    public static IServiceCollection CommerceRegisterMqListeners(this IServiceCollection services)
    {
        return services
            .RegisterMqListener<OrganizationSetLegalReceive, OrganizationLegalModel, TResponseModel<bool>>()
            .RegisterMqListener<OrganizationUpdateReceive, TAuthRequestModel<OrganizationModelDB>, TResponseModel<int>>()
            .RegisterMqListener<OrganizationsSelectReceive, TPaginationRequestAuthModel<OrganizationsSelectRequestModel>, TPaginationResponseModel<OrganizationModelDB>>()
            .RegisterMqListener<AddressOrganizationUpdateReceive, AddressOrganizationBaseModel, TResponseModel<int>>()
            .RegisterMqListener<AddressOrganizationDeleteReceive, int, ResponseBaseModel>()
            .RegisterMqListener<NomenclatureUpdateReceive, NomenclatureModelDB, TResponseModel<int>>()
            .RegisterMqListener<OrdersByIssuesGetReceive, OrdersByIssuesSelectRequestModel, TResponseModel<OrderDocumentModelDB[]>>()
            .RegisterMqListener<OfferDeleteReceive, int, TResponseModel<bool>>()
            .RegisterMqListener<UserOrganizationUpdateReceive, TAuthRequestModel<UserOrganizationModelDB>, TResponseModel<int>>()
            .RegisterMqListener<UsersOrganizationsReadReceive, int[], TResponseModel<UserOrganizationModelDB[]>>()
            .RegisterMqListener<UsersOrganizationsSelectReceive, TPaginationRequestAuthModel<UsersOrganizationsStatusesRequest>, TResponseModel<TPaginationResponseModel<UserOrganizationModelDB>>>()
            .RegisterMqListener<WeeklyScheduleUpdateReceive, WeeklyScheduleModelDB, TResponseModel<int>>()
            .RegisterMqListener<WeeklySchedulesSelectReceive, TPaginationRequestModel<WorkSchedulesSelectRequestModel>, TPaginationResponseModel<WeeklyScheduleModelDB>>()
            .RegisterMqListener<WeeklySchedulesReadReceive, int[], List<WeeklyScheduleModelDB>>()
            .RegisterMqListener<CalendarScheduleUpdateReceive, TAuthRequestModel<CalendarScheduleModelDB>, TResponseModel<int>>()
            .RegisterMqListener<CalendarsSchedulesSelectReceive, TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel>, TPaginationResponseModel<CalendarScheduleModelDB>>()
            .RegisterMqListener<CalendarsSchedulesReadReceive, int[], List<CalendarScheduleModelDB>>()
            .RegisterMqListener<PriceFullFileGetReceive, object, FileAttachModel>()
            .RegisterMqListener<OrderReportGetReceive, TAuthRequestModel<int>, TResponseModel<FileAttachModel>>()
            .RegisterMqListener<OffersRegistersSelectReceive, TPaginationRequestModel<RegistersSelectRequestBaseModel>, TPaginationResponseModel<OfferAvailabilityModelDB>>()
            .RegisterMqListener<WarehousesSelectReceive, TPaginationRequestModel<WarehouseDocumentsSelectRequestModel>, TPaginationResponseModel<WarehouseDocumentModelDB>>()
            .RegisterMqListener<WarehousesDocumentsReadReceive, int[], TResponseModel<WarehouseDocumentModelDB[]>>()
            .RegisterMqListener<WarehouseDocumentUpdateReceive, WarehouseDocumentModelDB, TResponseModel<int>>()
            .RegisterMqListener<RowsForWarehouseDocumentDeleteReceive, int[], TResponseModel<bool>>()
            .RegisterMqListener<RowForWarehouseDocumentUpdateReceive, RowOfWarehouseDocumentModelDB, TResponseModel<int>>()
            .RegisterMqListener<StatusChangeReceive, StatusChangeRequestModel, TResponseModel<bool>>()
            .RegisterMqListener<PriceRuleDeleteReceive, int, TResponseModel<bool>>()
            .RegisterMqListener<OrdersAttendancesByIssuesGetReceive, OrdersByIssuesSelectRequestModel, TResponseModel<OrderAttendanceModelDB[]>>()
            .RegisterMqListener<WorkSchedulesFindReceive, WorkSchedulesFindRequestModel, WorkSchedulesFindResponseModel>()
            .RegisterMqListener<PriceRuleUpdateReceive, PriceRuleForOfferModelDB, TResponseModel<int>>()
            .RegisterMqListener<PricesRulesGetForOffersReceive, int[], List<PriceRuleForOfferModelDB>>()
            .RegisterMqListener<PaymentDocumentUpdateReceive, PaymentDocumentBaseModel, TResponseModel<int>>()
            .RegisterMqListener<OrderUpdateReceive, OrderDocumentModelDB, TResponseModel<int>>()
            .RegisterMqListener<OffersReadReceive, int[], TResponseModel<OfferModelDB[]>>()
            .RegisterMqListener<NomenclaturesReadReceive, int[], List<NomenclatureModelDB>>()
            .RegisterMqListener<AddressesOrganizationsReadReceive, int[], TResponseModel<AddressOrganizationModelDB[]>>()
            .RegisterMqListener<PaymentDocumentDeleteReceive, int, ResponseBaseModel>()
            .RegisterMqListener<RowsForOrderDeleteReceive, int[], TResponseModel<bool>>()
            .RegisterMqListener<CreateAttendanceRecordsReceive, TAuthRequestModel<CreateAttendanceRequestModel>, ResponseBaseModel>()
            .RegisterMqListener<RowForOrderUpdateReceive, RowOfOrderDocumentModelDB, TResponseModel<int>>()
            .RegisterMqListener<OrdersReadReceive, int[], TResponseModel<OrderDocumentModelDB[]>>()
            .RegisterMqListener<OrganizationOfferContractUpdateReceive, TAuthRequestModel<OrganizationOfferToggleModel>, TResponseModel<bool>>()
            .RegisterMqListener<OrdersSelectReceive, TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>>, TPaginationResponseModel<OrderDocumentModelDB>>()
            .RegisterMqListener<OfferUpdateReceive, OfferModelDB, TResponseModel<int>>()
            .RegisterMqListener<OffersSelectReceive, TPaginationRequestModel<OffersSelectRequestModel>, TPaginationResponseModel<OfferModelDB>>()
            .RegisterMqListener<NomenclaturesSelectReceive, TPaginationRequestModel<NomenclaturesSelectRequestModel>, TPaginationResponseModel<NomenclatureModelDB>>()
            .RegisterMqListener<OrganizationsReadReceive, int[], TResponseModel<OrganizationModelDB[]>>()
            ;
    }
}