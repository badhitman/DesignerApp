////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// ReadDirectoriesReceive
/// </summary>
public class ReadDirectoriesReceive(IConstructorService conService) : IResponseReceive<int[]?, List<EntryNestedModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadDirectoriesReceive;

    /// <inheritdoc/>
    public async Task<List<EntryNestedModel>?> ResponseHandleAction(int[]? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.ReadDirectories(payload);
    }
}