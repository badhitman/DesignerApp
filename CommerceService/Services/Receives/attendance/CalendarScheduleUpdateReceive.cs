////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Обновление расписания на конкретную дату
/// </summary>
public class CalendarScheduleUpdateReceive(ICommerceService commerceRepo, ILogger<CalendarScheduleUpdateReceive> loggerRepo)
    : IResponseReceive<TAuthRequestModel<CalendarScheduleModelDB>?, TResponseModel<int>?>
{
    /// <summary>
    /// Обновление WorkScheduleCalendar
    /// </summary>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CalendarScheduleUpdateCommerceReceive;

    /// <summary>
    /// Обновление WorkScheduleCalendar
    /// </summary>
    public async Task<TResponseModel<int>?> ResponseHandleAction(TAuthRequestModel<CalendarScheduleModelDB>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(payload, GlobalStaticConstants.JsonSerializerSettings)}");
        return await commerceRepo.CalendarScheduleUpdate(payload);
    }
}