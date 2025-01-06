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
    : IResponseReceive<UserProjectModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CanEditProjectReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(UserProjectModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await conService.CanEditProject(req);
    }
}