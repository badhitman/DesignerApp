////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// CalendarsSchedulesReadReceive
/// </summary>
public class CalendarsSchedulesReadReceive(ICommerceService commerceRepo)
    : IResponseReceive<int[], TResponseModel<CalendarScheduleModelDB[]>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CalendarsSchedulesReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<CalendarScheduleModelDB[]>?> ResponseHandleAction(int[]? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await commerceRepo.CalendarsSchedulesRead(payload);
    }
}