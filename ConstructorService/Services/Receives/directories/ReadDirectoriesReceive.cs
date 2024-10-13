////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// ReadDirectoriesReceive
/// </summary>
public class ReadDirectoriesReceive(IConstructorService conService)
    : IResponseReceive<int[]?, EntryNestedModel[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadDirectoriesReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryNestedModel[]?>> ResponseHandleAction(int[]? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return new()
        {
            Response = [.. await conService.ReadDirectories(payload)],
        };
    }
}