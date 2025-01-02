////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// GetMembersOfProjectReceive
/// </summary>
public class GetMembersOfProjectReceive(IConstructorService conService)
    : IResponseReceive<int, EntryAltModel[]>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetMembersOfProjectReceive;

    /// <inheritdoc/>
    public async Task<EntryAltModel[]?> ResponseHandleAction(int req)
    {
        return await conService.GetMembersOfProject(req);
    }
}