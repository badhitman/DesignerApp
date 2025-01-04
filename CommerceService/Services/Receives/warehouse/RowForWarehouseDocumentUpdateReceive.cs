////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// RowForWarehouseDocumentUpdate
/// </summary>
public class RowForWarehouseDocumentUpdateReceive(ICommerceService commRepo, ILogger<RowForWarehouseDocumentUpdateReceive> loggerRepo)
    : IResponseReceive<RowOfWarehouseDocumentModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RowForWarehouseDocumentUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(RowOfWarehouseDocumentModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int> res = await commRepo.RowForWarehouseDocumentUpdate(req);
        return new() { Messages = res.Messages, Response = res.Response };
    }
}