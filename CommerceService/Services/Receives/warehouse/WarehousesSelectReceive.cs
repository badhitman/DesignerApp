////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// WarehousesSelectReceive
/// </summary>
public class WarehousesSelectReceive(ICommerceService commRepo) : IResponseReceive<TPaginationRequestModel<WarehouseDocumentsSelectRequestModel>?, TPaginationResponseModel<WarehouseDocumentModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WarehousesSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<WarehouseDocumentModelDB>?> ResponseHandleAction(TPaginationRequestModel<WarehouseDocumentsSelectRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commRepo.WarehouseDocumentsSelect(req);
    }
}