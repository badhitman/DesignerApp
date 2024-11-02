////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Set web config site
/// </summary>
public class SetWebConfigReceive(IOptions<WebConfigModel> webConfig, ILogger<SetWebConfigReceive> _logger)
    : IResponseReceive<WebConfigModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetWebConfigHelpdeskReceive;

    /// <inheritdoc/>
    public Task<TResponseModel<object?>> ResponseHandleAction(WebConfigModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(payload)}");
        TResponseModel<object?> res = new();
        ResponseBaseModel upd = webConfig.Value.Update(payload);
        res.AddRangeMessages(upd.Messages);
        return Task.FromResult(res);
    }
}