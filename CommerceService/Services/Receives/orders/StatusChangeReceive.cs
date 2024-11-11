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
public class StatusChangeReceive(ICommerceService commRepo, ILogger<StatusChangeReceive> LoggerRepo) : IResponseReceive<StatusChangeRequestModel?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.StatusChangeOrderReceive;
    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(StatusChangeRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        LoggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");

        TResponseModel<bool> res = await commRepo.StatusChange(req);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}