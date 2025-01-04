////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Rows for warehouse document delete
/// </summary>
public class RowsForWarehouseDocumentDeleteReceive(ICommerceService commRepo, ILogger<RowsForWarehouseDocumentDeleteReceive> loggerRepo)
    : IResponseReceive<int[]?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RowsDeleteFromWarehouseDocumentCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<bool> res = await commRepo.RowsForWarehouseDocumentDelete(req);

        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}