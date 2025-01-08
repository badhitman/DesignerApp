////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// AttendancesRecordsByIssuesGetReceive
/// </summary>
public class AttendancesRecordsByIssuesGetReceive(ICommerceService commRepo) : IResponseReceive<OrdersByIssuesSelectRequestModel?, TResponseModel<OrderAttendanceModelDB[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersAttendancesByIssuesGetReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderAttendanceModelDB[]>?> ResponseHandleAction(OrdersByIssuesSelectRequestModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await commRepo.AttendancesRecordsByIssuesGet(payload);
    }
}