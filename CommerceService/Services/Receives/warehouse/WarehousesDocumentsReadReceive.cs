////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// WarehousesDocumentsReadReceive
/// </summary>
public class WarehousesDocumentsReadReceive(ICommerceService commRepo)
: IResponseReceive<int[]?, WarehouseDocumentModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WarehousesDocumentsReadCommerceReceive;

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