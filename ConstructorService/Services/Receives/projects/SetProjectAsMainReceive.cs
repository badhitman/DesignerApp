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
    : IResponseReceive<UserProjectModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetProjectAsMainReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(UserProjectModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        ResponseBaseModel res = await conService.SetProjectAsMain(req);
        return new()
        {
            Messages = res.Messages,
        };
    }
}