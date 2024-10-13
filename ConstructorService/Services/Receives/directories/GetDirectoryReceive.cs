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
    : IResponseReceive<int?, EntryDescriptionModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetDirectoryReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryDescriptionModel?>> ResponseHandleAction(int? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        EntryDescriptionModel res = await conService.GetDirectory(payload.Value);
        return new()
        {
            Response = res,
        };
    }
}