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
            .RegisterMqListener<OrganizationSetLegalReceive, OrganizationLegalModel?, bool?>()
            .RegisterMqListener<OrganizationUpdateReceive, TAuthRequestModel<OrganizationModelDB>?, int?>()
            .RegisterMqListener<OrganizationsSelectReceive, TPaginationRequestAuthModel<OrganizationsSelectRequestModel>?, TPaginationResponseModel<OrganizationModelDB>?>()
            .RegisterMqListener<AddressOrganizationUpdateReceive, AddressOrganizationBaseModel?, int?>()
            .RegisterMqListener<AddressOrganizationDeleteReceive, int?, object?>()
            .RegisterMqListener<NomenclatureUpdateReceive, NomenclatureModelDB?, int?>()
            .RegisterMqListener<OrdersByIssuesGetReceive, OrdersByIssuesSelectRequestModel?, OrderDocumentModelDB[]?>()
            .RegisterMqListener<OfferDeleteReceive, int?, bool?>()
            .RegisterMqListener<StatusesOrdersAttendancesChangeByHelpdeskDocumentIdReceive, TAuthRequestModel<StatusChangeRequestModel>?, bool?>()
            .RegisterMqListener<UserOrganizationUpdateReceive, TAuthRequestModel<UserOrganizationModelDB>?, int?>()
            .RegisterMqListener<UsersOrganizationsReadReceive, int[]?, UserOrganizationModelDB[]?>()
            .RegisterMqListener<UsersOrganizationsSelectReceive, TPaginationRequestAuthModel<UsersOrganizationsStatusesRequest>?, TPaginationResponseModel<UserOrganizationModelDB>?>()
            .RegisterMqListener<WeeklyScheduleUpdateReceive, WeeklyScheduleModelDB?, int?>()
            .RegisterMqListener<WeeklySchedulesSelectReceive, TPaginationRequestModel<WorkSchedulesSelectRequestModel>?, TPaginationResponseModel<WeeklyScheduleModelDB>?>()
            .RegisterMqListener<WeeklySchedulesReadReceive, int[]?, WeeklyScheduleModelDB[]?>()
            .RegisterMqListener<CalendarScheduleUpdateReceive, CalendarScheduleModelDB?, int?>()
            .RegisterMqListener<CalendarsSchedulesSelectReceive, TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel>?, TPaginationResponseModel<CalendarScheduleModelDB>?>()
            .RegisterMqListener<CalendarsSchedulesReadReceive, int[]?, CalendarScheduleModelDB[]?>()
            .RegisterMqListener<PriceFullFileGetReceive, object?, FileAttachModel?>()
            .RegisterMqListener<OrderReportGetReceive, TAuthRequestModel<int>?, FileAttachModel?>()
            .RegisterMqListener<OffersRegistersSelectReceive, TPaginationRequestModel<RegistersSelectRequestBaseModel>?, TPaginationResponseModel<OfferAvailabilityModelDB>?>()
            .RegisterMqListener<WarehousesSelectReceive, TPaginationRequestModel<WarehouseDocumentsSelectRequestModel>?, TPaginationResponseModel<WarehouseDocumentModelDB>?>()
            .RegisterMqListener<WarehousesDocumentsReadReceive, int[]?, WarehouseDocumentModelDB[]?>()
            .RegisterMqListener<WarehouseDocumentUpdateReceive, WarehouseDocumentModelDB?, int?>()
            .RegisterMqListener<RowsForWarehouseDocumentDeleteReceive, int[]?, bool?>()
            .RegisterMqListener<RowForWarehouseDocumentUpdateReceive, RowOfWarehouseDocumentModelDB?, int?>()
            .RegisterMqListener<StatusChangeReceive, StatusChangeRequestModel?, bool?>()
            .RegisterMqListener<PriceRuleDeleteReceive, int?, bool?>()
            .RegisterMqListener<OrdersAttendancesByIssuesGetReceive, OrdersByIssuesSelectRequestModel?, OrderAttendanceModelDB[]?>()
            .RegisterMqListener<WorkSchedulesFindReceive, WorkSchedulesFindRequestModel?, WorkSchedulesFindResponseModel?>()
            .RegisterMqListener<PriceRuleUpdateReceive, PriceRuleForOfferModelDB?, int?>()
            .RegisterMqListener<PricesRulesGetForOffersReceive, int[]?, PriceRuleForOfferModelDB[]?>()
            .RegisterMqListener<PaymentDocumentUpdateReceive, PaymentDocumentBaseModel?, int?>()
            .RegisterMqListener<OrderUpdateReceive, OrderDocumentModelDB?, int?>()
            .RegisterMqListener<OffersReadReceive, int[]?, OfferModelDB[]?>()
            .RegisterMqListener<NomenclaturesReadReceive, int[]?, NomenclatureModelDB[]?>()
            .RegisterMqListener<AddressesOrganizationsReadReceive, int[]?, AddressOrganizationModelDB[]?>()
            .RegisterMqListener<PaymentDocumentDeleteReceive, int?, bool?>()
            .RegisterMqListener<RowsForOrderDeleteReceive, int[]?, bool?>()
            .RegisterMqListener<CreateAttendanceRecordsReceive, TAuthRequestModel<CreateAttendanceRequestModel>?, object?>()
            .RegisterMqListener<RowForOrderUpdateReceive, RowOfOrderDocumentModelDB?, int?>()
            .RegisterMqListener<OrdersReadReceive, int[]?, OrderDocumentModelDB[]?>()
            .RegisterMqListener<OrganizationOfferContractUpdateReceive, TAuthRequestModel<OrganizationOfferToggleModel>?, bool?>()
            .RegisterMqListener<OrdersSelectReceive, TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>>?, TPaginationResponseModel<OrderDocumentModelDB>?>()
            .RegisterMqListener<OfferUpdateReceive, OfferModelDB?, int?>()
            .RegisterMqListener<OffersSelectReceive, TPaginationRequestModel<OffersSelectRequestModel>?, TPaginationResponseModel<OfferModelDB>?>()
            .RegisterMqListener<NomenclaturesSelectReceive, TPaginationRequestModel<NomenclaturesSelectRequestModel>?, TPaginationResponseModel<NomenclatureModelDB>?>()
            .RegisterMqListener<OrganizationsReadReceive, int[]?, OrganizationModelDB[]?>();
    }
}