////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// TransmissionCommerceService
/// </summary>
public class TransmissionCommerceService(IRabbitClient rabbitClient) : ICommerceRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> AddressOrganizationDelete(int req)
        => await rabbitClient.MqRemoteCall<bool?>(GlobalStaticConstants.TransmissionQueues.AddressOrganizationDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> AddressOrganizationUpdate(AddressOrganizationBaseModel req)
        => await rabbitClient.MqRemoteCall<bool?>(GlobalStaticConstants.TransmissionQueues.AddressOrganizationUpdateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> DeliveryOrderUpdate(DeliveryForOrderUpdateRequestModel delivery)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.DeliveryOrderUpdateCommerceReceive, delivery);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<GoodModelDB>?>> GoodsSelect(TPaginationRequestModel<GoodsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<GoodModelDB>?>(GlobalStaticConstants.TransmissionQueues.GoodsSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> GoodUpdateReceive(GoodModelDB req)
        => await rabbitClient.MqRemoteCall<bool?>(GlobalStaticConstants.TransmissionQueues.GoodsUpdateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> OfferDelete(int req)
        => await rabbitClient.MqRemoteCall<bool?>(GlobalStaticConstants.TransmissionQueues.OfferDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OfferGoodModelDB>?>> OffersSelect(TPaginationRequestModel<OffersSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OfferGoodModelDB>?>(GlobalStaticConstants.TransmissionQueues.GoodsSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> OfferUpdate(OfferGoodModelDB offer)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.OfferUpdateCommerceReceive, offer);

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]?>> OrdersRead(int[] orders_ids)
        => await rabbitClient.MqRemoteCall<OrderDocumentModelDB[]?>(GlobalStaticConstants.TransmissionQueues.OrganizationsReadCommerceReceive, orders_ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> OrganizationSetLegal(OrganizationModelDB org)
        => await rabbitClient.MqRemoteCall<bool?>(GlobalStaticConstants.TransmissionQueues.OrganizationSetLegalCommerceReceive, org);

    /// <inheritdoc/>
    public async Task<TResponseModel<OrganizationModelDB[]?>> OrganizationsRead(int[] req)
        => await rabbitClient.MqRemoteCall<OrganizationModelDB[]?>(GlobalStaticConstants.TransmissionQueues.OrganizationsReadCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>?>> OrganizationsSelect(TPaginationRequestModel<OrganizationsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OrganizationModelDB>?>(GlobalStaticConstants.TransmissionQueues.OrganizationsSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> OrganizationUpdate(OrganizationModelDB org)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.OrganizationUpdateOrCreateCommerceReceive, org);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> PaymentDocumentDelete(int req)
        => await rabbitClient.MqRemoteCall<bool?>(GlobalStaticConstants.TransmissionQueues.PaymentDocumentDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> RowForOrderUpdate(RowOfOrderDocumentModelDB row)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.OrganizationUpdateOrCreateCommerceReceive, row);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> RowsForOrderDelete(int[] req)
        => await rabbitClient.MqRemoteCall<bool?>(GlobalStaticConstants.TransmissionQueues.RowsDeleteFromOrderCommerceReceive, req);
}