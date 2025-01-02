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
    : IResponseReceive<int, EntryDescriptionModel>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetDirectoryReceive;

    /// <inheritdoc/>
    public async Task<EntryDescriptionModel?> ResponseHandleAction(int payload)
    {
        return await conService.GetDirectory(payload);
    }
}