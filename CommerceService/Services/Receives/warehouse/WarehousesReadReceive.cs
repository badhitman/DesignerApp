////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// WarehousesReadReceive
/// </summary>
public class WarehousesReadReceive(ICommerceService commRepo)
: IResponseReceive<int[]?, WarehouseDocumentModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WarehousesReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<WarehouseDocumentModelDB[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        TResponseModel<WarehouseDocumentModelDB[]> res = await commRepo.WarehouseDocumentsRead(req);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}