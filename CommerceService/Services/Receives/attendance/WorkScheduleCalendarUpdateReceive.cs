////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Обновление WorkScheduleCalendar
/// </summary>
public class WorkScheduleCalendarUpdateReceive(ICommerceService commerceRepo, ILogger<WorkScheduleCalendarUpdateReceive> loggerRepo)
    : IResponseReceive<CalendarScheduleModelDB?, int?>
{
    /// <summary>
    /// Обновление WorkScheduleCalendar
    /// </summary>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorkScheduleCalendarUpdateCommerceReceive;

    /// <summary>
    /// Обновление WorkScheduleCalendar
    /// </summary>
    public async Task<TResponseModel<int?>> ResponseHandleAction(CalendarScheduleModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int> res = await commerceRepo.WorkScheduleCalendarUpdate(req);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}