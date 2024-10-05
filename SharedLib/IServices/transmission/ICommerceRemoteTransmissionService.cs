////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// E-Commerce Remote Transmission Service
/// </summary>
public interface ICommerceRemoteTransmissionService
{
    /// <summary>
    /// GetFileOrder
    /// </summary>
    public Task<TResponseModel<byte[]>> GetFileOrder(string id);

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
    /// OrderUpdate
    /// </summary>
    public Task<TResponseModel<int>> OrderUpdate(OrderDocumentModelDB order);

    /// <summary>
    /// PricesRulesGetForOffers
    /// </summary>
    public Task<TResponseModel<PriceRuleForOfferModelDB[]>> PricesRulesGetForOffers(int[] ids);

    /// <summary>
    /// OffersRead
    /// </summary>
    public Task<TResponseModel<OfferGoodModelDB[]>> OffersRead(int[] ids);

    /// <summary>
    /// GoodsRead
    /// </summary>
    public Task<TResponseModel<GoodsModelDB[]>> GoodsRead(int[] ids);

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
    /// Прикрепить файл к заказу (счёт, акт и т.п.)
    /// </summary>
    public Task<TResponseModel<int>> AttachmentForOrder(AttachmentForOrderRequestModel att);

    /// <summary>
    /// Обновить строку заказа
    /// </summary>
    public Task<TResponseModel<int>> RowForOrderUpdate(RowOfOrderDocumentModelDB row);

    /// <summary>
    /// OrdersRead
    /// </summary>
    public Task<TResponseModel<OrderDocumentModelDB[]>> OrdersRead(int[] orders_ids);

    /// <summary>
    /// Подбор (поиск по параметрам) заказов
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>> OrdersSelect(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req);

    /// <summary>
    /// Удалить Offer
    /// </summary>
    public Task<TResponseModel<bool>> OfferDelete(int req);

    /// <summary>
    /// OffersSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OfferGoodModelDB>>> OffersSelect(TPaginationRequestModel<OffersSelectRequestModel> req);

    /// <summary>
    /// GoodsSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<GoodsModelDB>>> GoodsSelect(TPaginationRequestModel<GoodsSelectRequestModel> req);

    /// <summary>
    /// Подбор организаций с параметрами запроса
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>>> OrganizationsSelect(TPaginationRequestModel<OrganizationsSelectRequestModel> req);

    /// <summary>
    /// OrganizationUpdate
    /// </summary>
    public Task<TResponseModel<int>> OfferUpdate(OfferGoodModelDB offer);

    /// <summary>
    /// Обновление параметров организации. Юридические параметры не меняются, а формируется запрос на изменение, которое должна подтвердить сторонняя система
    /// </summary>
    public Task<TResponseModel<int>> OrganizationUpdate(TAuthRequestModel<OrganizationModelDB> org);

    /// <summary>
    /// Установить реквизиты организации (+ сброс запроса редактирования)
    /// </summary>
    /// <remarks>
    /// Если организация находиться в статусе запроса изменения реквизитов - этот признак обнуляется.
    /// </remarks>
    public Task<TResponseModel<bool>> OrganizationSetLegal(OrganizationModelDB org);

    /// <summary>
    /// Прочитать данные организаций по их идентификаторам
    /// </summary>
    public Task<TResponseModel<OrganizationModelDB[]>> OrganizationsRead(int[] organizations_ids);

    /// <summary>
    /// Удалить адрес организации
    /// </summary>
    public Task<TResponseModel<bool>> AddressOrganizationDelete(int req);

    /// <summary>
    /// Обновить/Создать адрес организации
    /// </summary>
    public Task<TResponseModel<int>> AddressOrganizationUpdate(AddressOrganizationBaseModel req);

    /// <summary>
    /// Обновить/Создать товар
    /// </summary>
    public Task<TResponseModel<int>> GoodUpdateReceive(GoodsModelDB req);
}