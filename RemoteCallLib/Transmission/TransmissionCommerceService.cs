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
    public async Task<TResponseModel<bool>> AddressOrganizationDelete(int req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.AddressOrganizationDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> AddressOrganizationUpdate(AddressOrganizationBaseModel req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.AddressOrganizationUpdateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> AttachmentForOrder(AttachmentForOrderRequestModel att)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.AttachmentAddToOrderCommerceReceive, att);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> DeliveryOrderUpdate(DeliveryForOrderUpdateRequestModel delivery)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.DeliveryOrderUpdateCommerceReceive, delivery);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<GoodsModelDB>>> GoodsSelect(TPaginationRequestModel<GoodsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<GoodsModelDB>>(GlobalStaticConstants.TransmissionQueues.GoodsSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> GoodUpdateReceive(GoodsModelDB req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.GoodsUpdateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> OfferDelete(int req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.OfferDeleteCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OfferGoodModelDB>>> OffersSelect(TPaginationRequestModel<OffersSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OfferGoodModelDB>>(GlobalStaticConstants.TransmissionQueues.OfferSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OfferUpdate(OfferGoodModelDB offer)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.OfferUpdateCommerceReceive, offer);

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>> OrdersRead(int[] orders_ids)
        => await rabbitClient.MqRemoteCall<OrderDocumentModelDB[]>(GlobalStaticConstants.TransmissionQueues.OrdersReadCommerceReceive, orders_ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> OrganizationSetLegal(OrganizationModelDB org)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.OrganizationSetLegalCommerceReceive, org);

    /// <inheritdoc/>
    public async Task<TResponseModel<OrganizationModelDB[]>> OrganizationsRead(int[] req)
        => await rabbitClient.MqRemoteCall<OrganizationModelDB[]>(GlobalStaticConstants.TransmissionQueues.OrganizationsReadCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>>> OrganizationsSelect(TPaginationRequestModel<OrganizationsSelectRequestModel> req)
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
    public async Task<TResponseModel<GoodsModelDB[]>> GoodsRead(int[] ids)
        => await rabbitClient.MqRemoteCall<GoodsModelDB[]>(GlobalStaticConstants.TransmissionQueues.GoodsReadCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<OfferGoodModelDB[]>> OffersRead(int[] ids)
        => await rabbitClient.MqRemoteCall<OfferGoodModelDB[]>(GlobalStaticConstants.TransmissionQueues.OfferReadCommerceReceive, ids);

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
}