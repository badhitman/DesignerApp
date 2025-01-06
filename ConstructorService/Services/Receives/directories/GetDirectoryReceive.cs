////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// GetDirectoryReceive
/// </summary>
public class GetDirectoryReceive(IConstructorService conService)
    : IResponseReceive<int, TResponseModel<EntryDescriptionModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetDirectoryReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryDescriptionModel>?> ResponseHandleAction(int payload)
    {
        return await conService.GetDirectory(payload);
    }
}