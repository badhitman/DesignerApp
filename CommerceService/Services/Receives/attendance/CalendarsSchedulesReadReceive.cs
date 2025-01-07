////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// CalendarsSchedulesReadReceive
/// </summary>
public class CalendarsSchedulesReadReceive(ICommerceService commerceRepo) : IResponseReceive<int[]?, List<CalendarScheduleModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CalendarsSchedulesReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<List<CalendarScheduleModelDB>?> ResponseHandleAction(int[]? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await commerceRepo.CalendarSchedulesRead(payload);
    }
}