////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// GetProjectsForUserReceive
/// </summary>
public class GetProjectsForUserReceive(IConstructorService conService)
    : IResponseReceive<GetProjectsForUserRequestModel, ProjectViewModel[]>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ProjectsForUserReceive;

    /// <inheritdoc/>
    public async Task<ProjectViewModel[]?> ResponseHandleAction(GetProjectsForUserRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await conService.GetProjectsForUser(req);
    }
}