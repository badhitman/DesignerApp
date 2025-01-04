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
    : IResponseReceive<ProjectFindModel?, EntryModel[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetDirectoriesReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryModel[]?>> ResponseHandleAction(ProjectFindModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseStrictModel<EntryModel[]> res = await conService.GetDirectories(payload);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages,
        };
    }
}