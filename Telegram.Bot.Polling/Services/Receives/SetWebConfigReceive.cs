////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Set web config site
/// </summary>
public class SetWebConfigReceive(WebConfigModel webConfig)
    : IResponseReceive<WebConfigModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetWebConfigReceive;

    /// <inheritdoc/>
    public Task<TResponseModel<object?>> ResponseHandleAction(WebConfigModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<object?> res = new();
        ResponseBaseModel upd = webConfig.Update(payload);
        res.AddRangeMessages(upd.Messages);
        return Task.FromResult(res);
    }
}