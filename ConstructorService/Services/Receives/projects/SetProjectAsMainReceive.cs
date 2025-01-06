////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// SetProjectAsMainReceive
/// </summary>
public class SetProjectAsMainReceive(IConstructorService conService)
    : IResponseReceive<UserProjectModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetProjectAsMainReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(UserProjectModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await conService.SetProjectAsMain(req);
    }
}