////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// AttendancesRecordsByIssuesGetReceive
/// </summary>
public class AttendancesRecordsByIssuesGetReceive(ICommerceService commRepo) : IResponseReceive<OrdersByIssuesSelectRequestModel?, TResponseModel<RecordsAttendanceModelDB[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersAttendancesByIssuesGetReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<RecordsAttendanceModelDB[]>?> ResponseHandleAction(OrdersByIssuesSelectRequestModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await commRepo.RecordsAttendancesByIssuesGet(payload);
    }
}