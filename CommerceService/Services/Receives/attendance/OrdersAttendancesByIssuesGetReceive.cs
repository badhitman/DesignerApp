////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrdersAttendancesByIssuesGet - receive
/// </summary>
public class OrdersAttendancesByIssuesGetReceive(ICommerceService commRepo)
: IResponseReceive<OrdersByIssuesSelectRequestModel?, OrderAttendanceModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersAttendancesByIssuesGetReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderAttendanceModelDB[]?>> ResponseHandleAction(OrdersByIssuesSelectRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        TResponseModel<OrderAttendanceModelDB[]> res = await commRepo.OrdersAttendancesByIssuesGet(req);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages,
        };
    }
}