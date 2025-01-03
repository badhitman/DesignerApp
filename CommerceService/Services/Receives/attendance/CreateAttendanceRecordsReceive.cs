////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// CreateAttendanceRecords
/// </summary>
public class CreateAttendanceRecordsReceive(ICommerceService commerceRepo, ILogger<CalendarScheduleUpdateReceive> loggerRepo)
    : IResponseReceive<TAuthRequestModel<CreateAttendanceRequestModel>, ResponseBaseModel>
{
    /// <summary>
    /// Обновление WorkScheduleCalendar
    /// </summary>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CreateAttendanceRecordsCommerceReceive;

/// <summary>
    /// Обновление WorkScheduleCalendar
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(TAuthRequestModel<CreateAttendanceRequestModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(payload, GlobalStaticConstants.JsonSerializerSettings)}");
        return await commerceRepo.CreateAttendanceRecords(payload);
    }
}