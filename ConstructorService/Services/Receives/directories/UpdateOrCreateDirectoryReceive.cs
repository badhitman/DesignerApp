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
    : IResponseReceive<TAuthRequestModel<EntryConstructedModel>, TResponseStrictModel<int>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateOrCreateDirectoryReceive;

    /// <inheritdoc/>
    public async Task<TResponseStrictModel<int>?> ResponseHandleAction(TAuthRequestModel<EntryConstructedModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.UpdateOrCreateDirectory(payload);
    }
}