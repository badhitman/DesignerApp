////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// DeleteMembersFromProjectReceive
/// </summary>
public class DeleteMembersFromProjectReceive(IConstructorService conService)
    : IResponseReceive<UsersProjectModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.DeleteMembersFromProjectReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(UsersProjectModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return new()
        {
            Response = await conService.DeleteMembersFromProject(req)
        };
    }
}