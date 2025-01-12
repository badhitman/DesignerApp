////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

public partial class CommerceTransmission
{
    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<OfferAvailabilityModelDB>> OffersRegistersSelect(TPaginationRequestModel<RegistersSelectRequestBaseModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OfferAvailabilityModelDB>>(GlobalStaticConstants.TransmissionQueues.OffersRegistersSelectCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RowsForWarehouseDelete(int[] req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.RowsDeleteFromWarehouseDocumentCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> RowForWarehouseUpdate(RowOfWarehouseDocumentModelDB req)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.RowForWarehouseDocumentUpdateCommerceReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<WarehouseDocumentModelDB[]>> WarehousesRead(int[] ids)
        => await rabbitClient.MqRemoteCall<TResponseModel<WarehouseDocumentModelDB[]>>(GlobalStaticConstants.TransmissionQueues.WarehousesDocumentsReadCommerceReceive, ids) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> WarehouseUpdate(WarehouseDocumentModelDB document)
    {
        document.DeliveryDate = document.DeliveryDate.ToUniversalTime();
        return await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.WarehouseDocumentUpdateCommerceReceive, document) ?? new();
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<WarehouseDocumentModelDB>> WarehousesSelect(TPaginationRequestModel<WarehouseDocumentsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<WarehouseDocumentModelDB>>(GlobalStaticConstants.TransmissionQueues.WarehousesSelectCommerceReceive, req) ?? new();
}