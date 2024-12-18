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
    public Task<TResponseModel<FileAttachModel>> PriceFullFileGet();

    /// <summary>
    /// Order report get
    /// </summary>
    public Task<TResponseModel<FileAttachModel>> OrderReportGet(TAuthRequestModel<int> req);

    /// <summary>
    /// Status order change
    /// </summary>
    public Task<TResponseModel<bool>> StatusOrderChange(StatusOrderChangeRequestModel id, bool waitResponse = true);

    /// <summary>
    /// Удалить ценообразование
    /// </summary>
    public Task<TResponseModel<bool>> PriceRuleDelete(int id);

    /// <summary>
    /// Обновить/создать правило ценообразования
    /// </summary>
    public Task<TResponseModel<int>> PriceRuleUpdate(PriceRuleForOfferModelDB price_rule);

    /// <summary>
    /// Обновить/создать платёжный документ
    /// </summary>
    public Task<TResponseModel<int>> PaymentDocumentUpdate(PaymentDocumentBaseModel payment);

    /// <summary>
    /// PricesRulesGetForOffers
    /// </summary>
    public Task<TResponseModel<PriceRuleForOfferModelDB[]>> PricesRulesGetForOffers(int[] ids);

    /// <summary>
    /// OffersRead
    /// </summary>
    public Task<TResponseModel<OfferModelDB[]>> OffersRead(int[] ids);

    /// <summary>
    /// NomenclaturesRead
    /// </summary>
    public Task<TResponseModel<NomenclatureModelDB[]>> NomenclaturesRead(int[] ids);

    /// <summary>
    /// Прочитать данные адресов организаций по их идентификаторам
    /// </summary>
    public Task<TResponseModel<AddressOrganizationModelDB[]>> AddressesOrganizationsRead(int[] ids);

    /// <summary>
    /// AttachmentDeleteFromOrder
    /// </summary>
    public Task<TResponseModel<bool>> AttachmentDeleteFromOrder(int req);

    /// <summary>
    /// Удалить платёжный документ
    /// </summary>
    public Task<TResponseModel<bool>> PaymentDocumentDelete(int req);

    /// <summary>
    /// Удалить строку заказа
    /// </summary>
    public Task<TResponseModel<bool>> RowsForOrderDelete(int[] req);

    /// <summary>
    /// Обновить строку заказа
    /// </summary>
    public Task<TResponseModel<int>> RowForOrderUpdate(RowOfOrderDocumentModelDB row);

    /// <summary>
    /// OrdersRead
    /// </summary>
    public Task<TResponseModel<OrderDocumentModelDB[]>> OrdersRead(int[] orders_ids);

    /// <summary>
    /// OrderUpdate
    /// </summary>
    public Task<TResponseModel<int>> OrderUpdate(OrderDocumentModelDB order);

    /// <summary>
    /// Подбор заказов (поиск по параметрам)
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>> OrdersSelect(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req);

    /// <summary>
    /// Получить заказов (по заявкам)
    /// </summary>
    public Task<TResponseModel<OrderDocumentModelDB[]>> OrdersByIssues(OrdersByIssuesSelectRequestModel req);

    /// <summary>
    /// Удалить Offer
    /// </summary>
    public Task<TResponseModel<bool>> OfferDelete(int req);

    /// <summary>
    /// OffersSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OfferModelDB>>> OffersSelect(TPaginationRequestModel<OffersSelectRequestModel> req);

    /// <summary>
    /// NomenclaturesSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<NomenclatureModelDB>>> NomenclaturesSelect(TPaginationRequestModel<NomenclaturesSelectRequestModel> req);

    /// <summary>
    /// OrganizationUpdate
    /// </summary>
    public Task<TResponseModel<int>> OfferUpdate(OfferModelDB offer);

    /// <summary>
    /// Установить реквизиты организации (+ сброс запроса редактирования)
    /// </summary>
    /// <remarks>
    /// Если организация находиться в статусе запроса изменения реквизитов - этот признак обнуляется.
    /// </remarks>
    public Task<TResponseModel<bool>> OrganizationSetLegal(OrganizationLegalModel org);

    /// <summary>
    /// Удалить адрес организации
    /// </summary>
    public Task<ResponseBaseModel> AddressOrganizationDelete(int req);

    /// <summary>
    /// Обновить/Создать адрес организации
    /// </summary>
    public Task<TResponseModel<int>> AddressOrganizationUpdate(AddressOrganizationBaseModel req);

    /// <summary>
    /// Обновить/Создать товар
    /// </summary>
    public Task<TResponseModel<int>> NomenclatureUpdateReceive(NomenclatureModelDB req);

    /// <summary>
    /// Подбор организаций с параметрами запроса
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>>> OrganizationsSelect(TPaginationRequestAuthModel<UniversalSelectRequestModel> req);

    /// <summary>
    /// Обновление параметров организации. Юридические параметры не меняются, а формируется запрос на изменение, которое должна подтвердить сторонняя система
    /// </summary>
    public Task<TResponseModel<int>> OrganizationUpdate(TAuthRequestModel<OrganizationModelDB> org);

    /// <summary>
    /// Прочитать данные организаций по их идентификаторам
    /// </summary>
    public Task<TResponseModel<OrganizationModelDB[]>> OrganizationsRead(int[] organizations_ids);

    /// <summary>
    /// UsersOrganizationsSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<UserOrganizationModelDB>>> UsersOrganizationsSelect(TPaginationRequestAuthModel<UniversalSelectRequestModel> req);

    /// <summary>
    /// UserOrganizationUpdate
    /// </summary>
    public Task<TResponseModel<int>> UserOrganizationUpdate(TAuthRequestModel<UserOrganizationModelDB> org);

    /// <summary>
    /// UsersOrganizationsRead
    /// </summary>
    public Task<TResponseModel<UserOrganizationModelDB[]>> UsersOrganizationsRead(int[] organizations_ids);
}