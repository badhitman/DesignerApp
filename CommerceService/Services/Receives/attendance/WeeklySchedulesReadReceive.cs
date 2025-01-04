////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// WorkScheduleReadReceive
/// </summary>
public class WeeklySchedulesReadReceive(ICommerceService commerceRepo)
    : IResponseReceive<int[]?, List<WeeklyScheduleModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WeeklySchedulesReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<List<WeeklyScheduleModelDB>?> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commerceRepo.WeeklySchedulesRead(req);
    }
}