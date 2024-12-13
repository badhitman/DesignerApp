////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// WorkScheduleCalendarsSelectReceive
/// </summary>
public class WorkScheduleCalendarsSelectReceive(ICommerceService commerceRepo)
    : IResponseReceive<TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel>?, TPaginationResponseModel<WorkScheduleCalendarModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorkScheduleCalendarsSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<WorkScheduleCalendarModelDB>?>> ResponseHandleAction(TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        if (req.PageSize < 10)
            req.PageSize = 10;

        TResponseModel<TPaginationResponseModel<WorkScheduleCalendarModelDB>> wc = await commerceRepo.WorkScheduleCalendarsSelect(req);

        return new()
        {
            Response = wc.Response,
            Messages = wc.Messages
        };
    }
}