////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// AddMembersToProjectReceive
/// </summary>
public class AddMembersToProjectReceive(IConstructorService conService)
    : IResponseReceive<UsersProjectModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddMembersToProjectReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(UsersProjectModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        ResponseBaseModel res = await conService.AddMemberToProject(req);
        return new()
        {
            Messages = res.Messages,
        };
    }
}