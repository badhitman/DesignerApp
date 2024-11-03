////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Options;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Get web config - receive
/// </summary>
public class GetWebConfigReceive(IOptions<TelegramBotConfigModel> webConfig)
    : IResponseReceive<object?, TelegramBotConfigModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetWebConfigReceive;

    /// <inheritdoc/>
    public Task<TResponseModel<TelegramBotConfigModel?>> ResponseHandleAction(object? payload = null)
    {
        return Task.FromResult(new TResponseModel<TelegramBotConfigModel?>()
        {
            Response = webConfig.Value
        });
    }
}