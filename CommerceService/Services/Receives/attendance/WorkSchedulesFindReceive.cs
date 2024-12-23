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
    : IResponseReceive<WorkSchedulesFindRequestModel?, WeeklyScheduleModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorksSchedulesFindCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<WeeklyScheduleModelDB?>> ResponseHandleAction(WorkSchedulesFindRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        WeeklyScheduleModelDB ws = await commerceRepo.WorkSchedulesFind(req);

        return new()
        {
            Response = ws,
        };
    }
}