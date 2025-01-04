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
    : IResponseReceive<CreateProjectRequestModel?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ProjectCreateReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(CreateProjectRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<int> res = await conService.CreateProject(req);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages
        };
    }
}