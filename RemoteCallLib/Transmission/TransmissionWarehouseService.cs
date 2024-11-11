////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

public partial class TransmissionCommerceService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RowsForWarehouseDelete(int[] req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.RowsDeleteFromWarehouseDocumentCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> RowForWarehouseUpdate(RowOfWarehouseDocumentModelDB req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.WarehouseDocumentUpdateCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<WarehouseDocumentModelDB[]>> WarehousesRead(int[] ids)
        => await rabbitClient.MqRemoteCall<WarehouseDocumentModelDB[]>(GlobalStaticConstants.TransmissionQueues.WarehousesReadCommerceReceive, ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> WarehouseUpdate(WarehouseDocumentModelDB document)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.RowForWarehouseDocumentUpdateCommerceReceive, document);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<WarehouseDocumentModelDB>>> WarehousesSelect(TPaginationRequestModel<WarehouseDocumentsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<WarehouseDocumentModelDB>>(GlobalStaticConstants.TransmissionQueues.WarehousesSelectCommerceReceive, req);
}