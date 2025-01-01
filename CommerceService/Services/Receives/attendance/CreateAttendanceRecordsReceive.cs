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
    : IResponseReceive<TAuthRequestModel<CreateAttendanceRequestModel>?, object?>
{
    /// <summary>
    /// Обновление WorkScheduleCalendar
    /// </summary>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CreateAttendanceRecordsCommerceReceive;

    /// <summary>
    /// Обновление WorkScheduleCalendar
    /// </summary>
    public async Task<TResponseModel<object?>> ResponseHandleAction(TAuthRequestModel<CreateAttendanceRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        ResponseBaseModel res = await commerceRepo.CreateAttendanceRecords(req);
        return new()
        {
            Messages = res.Messages,             
        };
    }
}