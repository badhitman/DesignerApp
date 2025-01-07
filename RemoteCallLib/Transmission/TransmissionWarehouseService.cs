﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

public partial class TransmissionCommerceService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OfferAvailabilityModelDB>>> OffersRegistersSelect(TPaginationRequestModel<RegistersSelectRequestBaseModel> req)
        => await rabbitClient.MqRemoteCall< TResponseModel<TPaginationResponseModel<OfferAvailabilityModelDB>>>(GlobalStaticConstants.TransmissionQueues.OffersRegistersSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RowsForWarehouseDelete(int[] req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.RowsDeleteFromWarehouseDocumentCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> RowForWarehouseUpdate(RowOfWarehouseDocumentModelDB req)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.RowForWarehouseDocumentUpdateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<WarehouseDocumentModelDB[]>> WarehousesRead(int[] ids)
        => await rabbitClient.MqRemoteCall<TResponseModel<WarehouseDocumentModelDB[]>>(GlobalStaticConstants.TransmissionQueues.WarehousesDocumentsReadCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> WarehouseUpdate(WarehouseDocumentModelDB document)
    {
        document.DeliveryDate = document.DeliveryDate.ToUniversalTime();
        return await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.WarehouseDocumentUpdateCommerceReceive, document);
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<WarehouseDocumentModelDB>>> WarehousesSelect(TPaginationRequestModel<WarehouseDocumentsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall< TResponseModel<TPaginationResponseModel<WarehouseDocumentModelDB>>>(GlobalStaticConstants.TransmissionQueues.WarehousesSelectCommerceReceive, req);
}