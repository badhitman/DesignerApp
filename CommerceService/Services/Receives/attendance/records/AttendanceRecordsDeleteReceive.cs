////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// AttendanceRecordsDeleteReceive
/// </summary>
public class AttendanceRecordsDeleteReceive(ICommerceService commerceRepo, ILogger<AttendanceRecordsDeleteReceive> loggerRepo) : IResponseReceive<TAuthRequestModel<int>?, ResponseBaseModel?>
{
    /// <summary>
    /// Обновление WorkScheduleCalendar
    /// </summary>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AttendanceRecordDeleteCommerceReceive;

    /// <summary>
    /// Обновление WorkScheduleCalendar
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(TAuthRequestModel<int>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(payload, GlobalStaticConstants.JsonSerializerSettings)}");
        return await commerceRepo.OrderAttendanceDeleteRecord(payload);
    }
}