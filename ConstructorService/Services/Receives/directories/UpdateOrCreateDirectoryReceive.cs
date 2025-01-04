////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// UpdateOrCreateDirectoryReceive
/// </summary>
public class UpdateOrCreateDirectoryReceive(IConstructorService conService)
    : IResponseReceive<TAuthRequestModel<EntryConstructedModel>?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateOrCreateDirectoryReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(TAuthRequestModel<EntryConstructedModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseStrictModel<int> res = await conService.UpdateOrCreateDirectory(payload);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages,
        };
    }
}