////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// WorkSchedulesSelectReceive
/// </summary>
public class WeekSchedulesSelectReceive(ICommerceService commerceRepo) 
    : IResponseReceive<TPaginationRequestModel<WorkSchedulesSelectRequestModel>?, TPaginationResponseModel<WeeklyScheduleModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorkSchedulesSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<WeeklyScheduleModelDB>?>> ResponseHandleAction(TPaginationRequestModel<WorkSchedulesSelectRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<TPaginationResponseModel<WeeklyScheduleModelDB>> ws = await commerceRepo.WeeklySchedulesSelect(req);

        return new()
        {
            Response = ws.Response,
            Messages = ws.Messages,
        };
    }
}