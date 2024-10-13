////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// UpdateProjectReceive
/// </summary>
public class UpdateProjectReceive(IConstructorService conService)
    : IResponseReceive<ProjectViewModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateProjectReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(ProjectViewModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        ResponseBaseModel res = await conService.UpdateProject(req);
        return new()
        {
            Messages = res.Messages
        };
    }
}