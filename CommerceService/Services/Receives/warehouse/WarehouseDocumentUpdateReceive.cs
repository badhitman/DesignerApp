////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// WarehouseDocumentUpdateReceive
/// </summary>
public class WarehouseDocumentUpdateReceive(ICommerceService commRepo, ILogger<WarehouseDocumentUpdateReceive> loggerRepo)
    : IResponseReceive<WarehouseDocumentModelDB, TResponseModel<int>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WarehouseDocumentUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int>?> ResponseHandleAction(WarehouseDocumentModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        return await commRepo.WarehouseDocumentUpdate(req);
    }
}