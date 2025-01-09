////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrdersReadReceive
/// </summary>
public class OrdersReadReceive(ICommerceService commRepo)
    : IResponseReceive<TAuthRequestModel<int[]>?, TResponseModel<OrderDocumentModelDB[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>?> ResponseHandleAction(TAuthRequestModel<int[]>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commRepo.OrdersRead(req);
    }
}