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
    : IResponseReceive<WorkSchedulesFindRequestModel?, WorkSchedulesFindResponseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorksSchedulesFindCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<WorkSchedulesFindResponseModel?>> ResponseHandleAction(WorkSchedulesFindRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<WorkSchedulesFindResponseModel> ws = await commerceRepo.WorkSchedulesFind(req);

        return new()
        {
            Response = ws.Response,
            Messages = ws.Messages,
        };
    }
}