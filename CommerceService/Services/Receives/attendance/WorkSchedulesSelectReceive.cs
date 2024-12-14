////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// WorkSchedulesSelectReceive
/// </summary>
public class WorkSchedulesSelectReceive(ICommerceService commerceRepo) 
    : IResponseReceive<TPaginationRequestModel<WorkSchedulesSelectRequestModel>?, TPaginationResponseModel<WorkScheduleModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorkSchedulesSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<WorkScheduleModelDB>?>> ResponseHandleAction(TPaginationRequestModel<WorkSchedulesSelectRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<TPaginationResponseModel<WorkScheduleModelDB>> ws = await commerceRepo.WorkSchedulesSelect(req);

        return new()
        {
            Response = ws.Response,
            Messages = ws.Messages,
        };
    }
}