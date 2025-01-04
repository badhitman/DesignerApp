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
    : IResponseReceive<OrdersByIssuesSelectRequestModel?, TResponseModel<OrderAttendanceModelDB[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersAttendancesByIssuesGetReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderAttendanceModelDB[]>?> ResponseHandleAction(OrdersByIssuesSelectRequestModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await commRepo.OrdersAttendancesByIssuesGet(payload);
    }
}