////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// SetMarkerDeleteProjectReceive
/// </summary>
public class SetMarkerDeleteProjectReceive(IConstructorService conService) 
    : IResponseReceive<SetMarkerProjectRequestModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetMarkerDeleteProjectReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(SetMarkerProjectRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        ResponseBaseModel res = await conService.SetMarkerDeleteProject(req);
        return new()
        {
            Messages = res.Messages
        };
    }
}