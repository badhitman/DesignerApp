////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// WorkSchedulesFindReceive
/// </summary>
public class WorkSchedulesFindReceive(ICommerceService commerceRepo) 
    : IResponseReceive<WorkSchedulesFindRequestModel?, WorkSchedulesViewModel[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorksSchedulesFindCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<WorkSchedulesViewModel[]?>> ResponseHandleAction(WorkSchedulesFindRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<WorkSchedulesViewModel[]> ws = await commerceRepo.WorkSchedulesFind(req);

        return new()
        {
            Response = ws.Response,
            Messages = ws.Messages,
        };
    }
}