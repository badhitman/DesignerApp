////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// WorkScheduleReadReceive
/// </summary>
public class WorkScheduleReadReceive(ICommerceService commerceRepo)
: IResponseReceive<int[]?, WeeklyScheduleModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorkSchedulesReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<WeeklyScheduleModelDB[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<WeeklyScheduleModelDB[]> wc = await commerceRepo.WorkSchedulesRead(req);
        return new()
        {
            Response = wc.Response,
            Messages = wc.Messages,
        };
    }
}