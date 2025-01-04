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
    : IResponseReceive<UsersProjectModel, ResponseBaseModel>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddMembersToProjectReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(UsersProjectModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await conService.AddMemberToProject(req);
    }
}