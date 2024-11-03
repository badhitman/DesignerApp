////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Set web config site
/// </summary>
public class SetWebConfigReceive(TelegramBotConfigModel webConfig, ILogger<SetWebConfigReceive> _logger)
    : IResponseReceive<TelegramBotConfigModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetWebConfigTelegramReceive;

    /// <inheritdoc/>
    public Task<TResponseModel<object?>> ResponseHandleAction(TelegramBotConfigModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(payload)}");
        TResponseModel<object?> res = new();
        ResponseBaseModel upd = webConfig.Update(payload);
        res.AddRangeMessages(upd.Messages);
        return Task.FromResult(res);
    }
}