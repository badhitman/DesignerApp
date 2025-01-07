////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// RowsForOrderDeleteReceive
/// </summary>
public class RowsForOrderDeleteReceive(ICommerceService commRepo, ILogger<RowsForOrderDeleteReceive> loggerRepo) : IResponseReceive<int[]?, TResponseModel<bool>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RowsDeleteFromOrderCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>?> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        return await commRepo.RowsForOrderDelete(req);
    }
}