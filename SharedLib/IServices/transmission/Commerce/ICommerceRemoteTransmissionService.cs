////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// E-Commerce Remote Transmission Service
/// </summary>
public partial interface ICommerceRemoteTransmissionService
{
    /// <summary>
    /// Price Full - file get
    /// </summary>
    public Task<FileAttachModel> PriceFullFileGet();

    /// <summary>
    /// Order report get
    /// </summary>
    public Task<FileAttachModel> OrderReportGet(TAuthRequestModel<int> req);

    /// <summary>
    /// Status order change
    /// </summary>
    public Task<bool> StatusOrderChange(StatusChangeRequestModel id, bool waitResponse = true);

    /// <summary>
    /// Удалить ценообразование
    /// </summary>
    public Task<bool> PriceRuleDelete(int id);

    /// <summary>
    /// Обновить/создать правило ценообразования
    /// </summary>
    public Task<int> PriceRuleUpdate(PriceRuleForOfferModelDB price_rule);

    /// <summary>
    /// Обновить/создать платёжный документ
    /// </summary>
    public Task<int> PaymentDocumentUpdate(PaymentDocumentBaseModel payment);

    /// <summary>
    /// PricesRulesGetForOffers
    /// </summary>
    public Task<PriceRuleForOfferModelDB[]> PricesRulesGetForOffers(int[] ids);

    /// <summary>
    /// OffersRead
    /// </summary>
    public Task<OfferModelDB[]> OffersRead(int[] ids);

    /// <summary>
    /// NomenclaturesRead
    /// </summary>
    public Task<NomenclatureModelDB[]> NomenclaturesRead(int[] ids);

    /// <summary>
    /// Прочитать данные адресов организаций по их идентификаторам
    /// </summary>
    public Task<AddressOrganizationModelDB[]> AddressesOrganizationsRead(int[] ids);

    /// <summary>
    /// AttachmentDeleteFromOrder
    /// </summary>
    public Task<bool> AttachmentDeleteFromOrder(int req);

    /// <summary>
    /// Удалить платёжный документ
    /// </summary>
    public Task<bool> PaymentDocumentDelete(int req);

    /// <summary>
    /// Удалить строку заказа
    /// </summary>
    public Task<bool> RowsForOrderDelete(int[] req);

    /// <summary>
    /// Обновить строку заказа
    /// </summary>
    public Task<int> RowForOrderUpdate(RowOfOrderDocumentModelDB row);

    /// <summary>
    /// OrdersRead
    /// </summary>
    public Task<OrderDocumentModelDB[]> OrdersRead(int[] orders_ids);

    /// <summary>
    /// OrderUpdate
    /// </summary>
    public Task<int> OrderUpdate(OrderDocumentModelDB order);

    /// <summary>
    /// Подбор заказов (поиск по параметрам)
    /// </summary>
    public Task<TPaginationResponseModel<OrderDocumentModelDB>> OrdersSelect(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req);

    /// <summary>
    /// Получить заказы (по заявкам)
    /// </summary>
    public Task<OrderDocumentModelDB[]> OrdersByIssues(OrdersByIssuesSelectRequestModel req);

    /// <summary>
    /// Удалить Offer
    /// </summary>
    public Task<bool> OfferDelete(int req);

    /// <summary>
    /// OffersSelect
    /// </summary>
    public Task<TPaginationResponseModel<OfferModelDB>> OffersSelect(TPaginationRequestModel<OffersSelectRequestModel> req);

    /// <summary>
    /// NomenclaturesSelect
    /// </summary>
    public Task<TPaginationResponseModel<NomenclatureModelDB>> NomenclaturesSelect(TPaginationRequestModel<NomenclaturesSelectRequestModel> req);

    /// <summary>
    /// OrganizationUpdate
    /// </summary>
    public Task<int> OfferUpdate(OfferModelDB offer);

    /// <summary>
    /// Установить реквизиты организации (+ сброс запроса редактирования)
    /// </summary>
    /// <remarks>
    /// Если организация находиться в статусе запроса изменения реквизитов - этот признак обнуляется.
    /// </remarks>
    public Task<bool> OrganizationSetLegal(OrganizationLegalModel org);

    /// <summary>
    /// Удалить адрес организации
    /// </summary>
    public Task<ResponseBaseModel> AddressOrganizationDelete(int req);

    /// <summary>
    /// Обновить/Создать адрес организации
    /// </summary>
    public Task<int> AddressOrganizationUpdate(AddressOrganizationBaseModel req);

    /// <summary>
    /// Обновить/Создать товар
    /// </summary>
    public Task<int> NomenclatureUpdateReceive(NomenclatureModelDB req);

    /// <summary>
    /// Подбор организаций с параметрами запроса
    /// </summary>
    public Task<TPaginationResponseModel<OrganizationModelDB>> OrganizationsSelect(TPaginationRequestAuthModel<OrganizationsSelectRequestModel> req);

    /// <summary>
    /// Обновление параметров организации. Юридические параметры не меняются, а формируется запрос на изменение, которое должна подтвердить сторонняя система
    /// </summary>
    public Task<int> OrganizationUpdate(TAuthRequestModel<OrganizationModelDB> org);

    /// <summary>
    /// Прочитать данные организаций по их идентификаторам
    /// </summary>
    public Task<OrganizationModelDB[]> OrganizationsRead(int[] organizations_ids);

    /// <summary>
    /// UsersOrganizationsSelect
    /// </summary>
    public Task<TPaginationResponseModel<UserOrganizationModelDB>> UsersOrganizationsSelect(TPaginationRequestAuthModel<UsersOrganizationsStatusesRequest> req);

    /// <summary>
    /// UserOrganizationUpdate
    /// </summary>
    public Task<int> UserOrganizationUpdate(TAuthRequestModel<UserOrganizationModelDB> org);

    /// <summary>
    /// UsersOrganizationsRead
    /// </summary>
    public Task<UserOrganizationModelDB[]> UsersOrganizationsRead(int[] organizations_ids);
}