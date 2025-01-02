////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrdersByIssuesGetReceive
/// </summary>
public class OrdersByIssuesGetReceive(ICommerceService commRepo)
: IResponseReceive<OrdersByIssuesSelectRequestModel?, OrderDocumentModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersByIssuesGetReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]?>> ResponseHandleAction(OrdersByIssuesSelectRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        TResponseModel<OrderDocumentModelDB[]> res = await commRepo.OrdersByIssuesGet(req);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages,
        };
    }
}

/// <summary>
/// 
/// </summary>
public class OrdersAttendancesByIssuesGetReceive(ICommerceService commRepo)
: IResponseReceive<OrdersByIssuesSelectRequestModel?, OrderDocumentModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersAttendancesByIssuesGetReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]?>> ResponseHandleAction(OrdersByIssuesSelectRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        TResponseModel<OrderDocumentModelDB[]> res = await commRepo.OrdersByIssuesGet(req);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages,
        };
    }
}