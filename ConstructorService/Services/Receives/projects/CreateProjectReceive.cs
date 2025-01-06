////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// CreateProjectReceive
/// </summary>
public class CreateProjectReceive(IConstructorService conService)
    : IResponseReceive<CreateProjectRequestModel?, TResponseModel<int>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ProjectCreateReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int>?> ResponseHandleAction(CreateProjectRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await conService.CreateProject(req);
    }
}