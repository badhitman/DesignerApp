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
    /// PaymentDocumentDelete
    /// </summary>
    public Task<TResponseModel<bool?>> PaymentDocumentDelete(int req);

    /// <summary>
    /// RowsForOrderDelete
    /// </summary>
    public Task<TResponseModel<bool?>> RowsForOrderDelete(int[] req);

    /// <summary>
    /// RowForOrderUpdate
    /// </summary>
    public Task<TResponseModel<int?>> RowForOrderUpdate(RowOfOrderDocumentModelDB row);

    /// <summary>
    /// DeliveryOrderUpdate
    /// </summary>
    public Task<TResponseModel<int?>> DeliveryOrderUpdate(DeliveryForOrderUpdateRequestModel delivery);

    /// <summary>
    /// OrdersRead
    /// </summary>
    public Task<TResponseModel<OrderDocumentModelDB[]?>> OrdersRead(int[] orders_ids);

    /// <summary>
    /// Удалить Offer
    /// </summary>
    public Task<TResponseModel<bool?>> OfferDelete(int req);

    /// <summary>
    /// OffersSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OfferGoodModelDB>?>> OffersSelect(TPaginationRequestModel<OffersSelectRequestModel> req);

    /// <summary>
    /// GoodsSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<GoodModelDB>?>> GoodsSelect(TPaginationRequestModel<GoodsSelectRequestModel> req);

    /// <summary>
    /// OrganizationsSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>?>> OrganizationsSelect(TPaginationRequestModel<OrganizationsSelectRequestModel> req);

    /// <summary>
    /// OrganizationUpdate
    /// </summary>
    public Task<TResponseModel<int?>> OfferUpdate(OfferGoodModelDB offer);

    /// <summary>
    /// OrganizationUpdate
    /// </summary>
    public Task<TResponseModel<int?>> OrganizationUpdate(OrganizationModelDB org);

    /// <summary>
    /// OrganizationSetLegal
    /// </summary>
    public Task<TResponseModel<bool?>> OrganizationSetLegal(OrganizationModelDB org);

    /// <summary>
    /// OrganizationsRead
    /// </summary>
    public Task<TResponseModel<OrganizationModelDB[]?>> OrganizationsRead(int[] org);

    /// <summary>
    /// Удалить адрес организации
    /// </summary>
    public Task<TResponseModel<bool?>> AddressOrganizationDelete(int req);

    /// <summary>
    /// Обновить/Создать адрес организации
    /// </summary>
    public Task<TResponseModel<bool?>> AddressOrganizationUpdate(AddressOrganizationBaseModel req);

    /// <summary>
    /// Обновить/Создать товар
    /// </summary>
    public Task<TResponseModel<bool?>> GoodUpdateReceive(GoodModelDB req);
}