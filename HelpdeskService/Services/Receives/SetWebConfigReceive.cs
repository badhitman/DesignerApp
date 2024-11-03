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
public class SetWebConfigReceive(IOptions<HelpdeskConfigModel> webConfig, ILogger<SetWebConfigReceive> _logger)
    : IResponseReceive<HelpdeskConfigModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetWebConfigHelpdeskReceive;

    /// <inheritdoc/>
    public Task<TResponseModel<object?>> ResponseHandleAction(HelpdeskConfigModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(payload)}");
        TResponseModel<object?> res = new();

        if(!Uri.TryCreate(payload.BaseUri, UriKind.Absolute,out _))
        {
            res.AddError("BaseUri is null");
            return Task.FromResult(res);
        }

        ResponseBaseModel upd = webConfig.Value.Update(payload.BaseUri);
        res.AddRangeMessages(upd.Messages);
        return Task.FromResult(res);
    }
}