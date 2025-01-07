////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrdersByIssuesGetReceive
/// </summary>
public class OrdersByIssuesGetReceive(ICommerceService commRepo) : IResponseReceive<OrdersByIssuesSelectRequestModel?, TResponseModel<OrderDocumentModelDB[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersByIssuesGetReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>?> ResponseHandleAction(OrdersByIssuesSelectRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commRepo.OrdersByIssuesGet(req);
    }
}