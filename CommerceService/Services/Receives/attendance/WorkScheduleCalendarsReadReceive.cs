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
    : IResponseReceive<int[]?, WorkScheduleCalendarModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorkScheduleCalendarsReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<WorkScheduleCalendarModelDB[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<WorkScheduleCalendarModelDB[]> wc = await commerceRepo.WorkScheduleCalendarsRead(req);
        return new()
        {
            Response = wc.Response,
            Messages = wc.Messages
        };
    }
}