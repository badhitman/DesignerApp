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
    : IResponseReceive<SetMarkerProjectRequestModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetMarkerDeleteProjectReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(SetMarkerProjectRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await conService.SetMarkerDeleteProject(req);
    }
}