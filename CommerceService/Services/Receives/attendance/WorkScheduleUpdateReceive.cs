////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Обновление WorkSchedule
/// </summary>
public class WorkScheduleUpdateReceive(ICommerceService commerceRepo, ILogger<WorkScheduleUpdateReceive> loggerRepo)
    : IResponseReceive<WeeklyScheduleModelDB?, int?>
{
    /// <summary>
    /// Обновление WorkSchedule
    /// </summary>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.WorkScheduleUpdateCommerceReceive;

    /// <summary>
    /// Обновление WorkSchedule
    /// </summary>
    public async Task<TResponseModel<int?>> ResponseHandleAction(WeeklyScheduleModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int> res = await commerceRepo.WorkScheduleUpdate(req);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}