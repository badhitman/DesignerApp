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
: IResponseReceive<int[]?, WorkScheduleModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorkSchedulesReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<WorkScheduleModelDB[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<WorkScheduleModelDB[]> wc = await commerceRepo.WorkSchedulesRead(req);
        return new()
        {
            Response = wc.Response,
            Messages = wc.Messages,
        };
    }
}