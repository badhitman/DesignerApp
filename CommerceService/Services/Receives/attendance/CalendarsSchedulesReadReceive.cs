////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// CalendarsSchedulesReadReceive
/// </summary>
public class CalendarsSchedulesReadReceive(ICommerceService commerceRepo) : IResponseReceive<TAuthRequestModel<int[]>?, TResponseModel<List<CalendarScheduleModelDB>>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CalendarsSchedulesReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<List<CalendarScheduleModelDB>>?> ResponseHandleAction(TAuthRequestModel<int[]>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await commerceRepo.CalendarSchedulesRead(payload);
    }
}