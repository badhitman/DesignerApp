////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// StatusChangeReceive
/// </summary>
public class StatusChangeReceive(ICommerceService commRepo, ILogger<StatusChangeReceive> LoggerRepo) : IResponseReceive<StatusOrderChangeRequestModel?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.StatusChangeOrderReceive;
    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(StatusOrderChangeRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        LoggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");

        TResponseModel<bool> res = await commRepo.StatusOrderChange(req);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}