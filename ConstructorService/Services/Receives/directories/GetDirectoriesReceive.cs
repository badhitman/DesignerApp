////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// GetDirectoriesReceive
/// </summary>
public class GetDirectoriesReceive(IConstructorService conService)
    : IResponseReceive<ProjectFindModel, TResponseStrictModel<EntryModel[]>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetDirectoriesReceive;

    /// <inheritdoc/>
    public async Task<TResponseStrictModel<EntryModel[]>?> ResponseHandleAction(ProjectFindModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.GetDirectories(payload);
    }
}