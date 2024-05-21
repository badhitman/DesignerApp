using Microsoft.Extensions.Logging;
using SharedLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Set web config site
/// </summary>
public class SetWebConfigReceive(WebConfigModel webConfig, ILogger<SetWebConfigReceive> _logger)
    : IResponseReceive<WebConfigModel?, object?>
{
    /// <inheritdoc/>
    public Task<TResponseModel<object?>> ResponseHandleAction(WebConfigModel? payload)
    {
        TResponseModel<object?> res = new();
        string msg;
        if (payload is null)
        {
            msg = $"remote call [payload] is null: error {{DD91F267-FA63-4351-86B6-31C8A7C9A855}}";
            res.AddError(msg);
            _logger.LogError(msg);
            return Task.FromResult(res);
        }

        ResponseBaseModel upd = webConfig.Update(payload);
        if (upd.Messages.Count != 0)
            res.AddRangeMessages(upd.Messages);

        return Task.FromResult(res);
    }
}