using Microsoft.Extensions.Options;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Get web config - receive
/// </summary>
public class GetWebConfigReceive(IOptions<WebConfigModel> webConfig)
    : IResponseReceive<object?, WebConfigModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetWebConfigReceive;

    /// <inheritdoc/>
    public Task<TResponseModel<WebConfigModel?>> ResponseHandleAction(object? payload = null)
    {
        return Task.FromResult(new TResponseModel<WebConfigModel?>()
        {
            Response = webConfig.Value
        });
    }
}