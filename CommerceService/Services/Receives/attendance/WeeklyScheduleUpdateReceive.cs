////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Обновление WeeklyScheduleUpdateReceive
/// </summary>
public class WeeklyScheduleUpdateReceive(ICommerceService commerceRepo, ILogger<WeeklyScheduleUpdateReceive> loggerRepo)
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
        TResponseModel<int> res = await commerceRepo.WeeklyScheduleUpdate(req);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}