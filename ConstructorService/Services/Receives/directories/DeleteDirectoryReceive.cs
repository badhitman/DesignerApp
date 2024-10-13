////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// DeleteDirectoryReceive
/// </summary>
public class DeleteDirectoryReceive(IConstructorService conService)
    : IResponseReceive<TAuthRequestModel<int>?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.DeleteDirectoryReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(TAuthRequestModel<int>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.DeleteDirectory(payload);
        return new()
        {
            Messages = res.Messages,
        };
    }
}