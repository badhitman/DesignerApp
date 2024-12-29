////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Options;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Get TelegramBot Token
/// </summary>
public class GetBotTokenReceive(IOptions<BotConfiguration> tgConfig, ILogger<GetBotTokenReceive> _logger)
    : IResponseReceive<object?, string?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetBotTokenTelegramReceive;

    /// <inheritdoc/>
    public Task<TResponseModel<string?>> ResponseHandleAction(object? payload)
    {
        _logger.LogInformation($"call `{GetType().Name}`");
        TResponseModel<string?> res = new() { Response = tgConfig.Value.BotToken };
        return Task.FromResult(res);
    }
}