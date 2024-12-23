////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// WorkScheduleCalendarsReadReceive
/// </summary>
public class WorkScheduleCalendarsReadReceive(ICommerceService commerceRepo) 
    : IResponseReceive<int[]?, CalendarScheduleModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorkScheduleCalendarsReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<CalendarScheduleModelDB[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<CalendarScheduleModelDB[]> wc = await commerceRepo.WorkScheduleCalendarsRead(req);
        return new()
        {
            Response = wc.Response,
            Messages = wc.Messages
        };
    }
}