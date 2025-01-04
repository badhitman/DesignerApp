////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// ProjectsReadReceive
/// </summary>
public class ProjectsReadReceive(IConstructorService conService)
: IResponseReceive<int[]?, ProjectModelDb[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ProjectsReadReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<ProjectModelDb[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return new() { Response = await conService.ReadProjects(req) };
    }
}