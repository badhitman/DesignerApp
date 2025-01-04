////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// GetCurrentMainProjectReceive
/// </summary>
public class GetCurrentMainProjectReceive(IConstructorService conService)
    : IResponseReceive<string, TResponseModel<MainProjectViewModel>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetCurrentMainProjectReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<MainProjectViewModel>?> ResponseHandleAction(string? req)
    {
        if (string.IsNullOrWhiteSpace(req))
            throw new ArgumentNullException(nameof(req));

        return await conService.GetCurrentMainProject(req);
    }
}