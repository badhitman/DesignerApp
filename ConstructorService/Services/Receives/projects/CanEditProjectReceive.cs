////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// CanEditProjectReceive
/// </summary>
public class CanEditProjectReceive(IConstructorService conService)
    : IResponseReceive<UserProjectModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CanEditProjectReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(UserProjectModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return new()
        {
            Response = await conService.CanEditProject(req)
        };
    }
}