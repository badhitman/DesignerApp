////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// WorksFindReceive
/// </summary>
public class WorksFindReceive(ICommerceService commerceRepo) : IResponseReceive<WorkFindRequestModel?, WorksFindResponseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorksSchedulesFindCommerceReceive;

    /// <inheritdoc/>
    public async Task<WorksFindResponseModel?> ResponseHandleAction(WorkFindRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commerceRepo.WorkSchedulesFind(req);
    }
}