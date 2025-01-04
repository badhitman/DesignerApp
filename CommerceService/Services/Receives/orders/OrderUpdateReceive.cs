////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrderUpdateReceive
/// </summary>
public class OrderUpdateReceive(ICommerceService commRepo, ILogger<OrderUpdateReceive> loggerRepo)
    : IResponseReceive<OrderDocumentModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrderUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(OrderDocumentModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<int> res = await commRepo.OrderUpdate(req);

        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}