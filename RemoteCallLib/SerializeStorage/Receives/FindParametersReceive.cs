////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Find parameters
/// </summary>
public class FindParametersReceive
    : IResponseReceive<RequestStorageCloudParameterModel?, FoundParameterModel[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FindCloudParameterReceive;

    /// <inheritdoc/>
    public Task<TResponseModel<FoundParameterModel[]?>> ResponseHandleAction(RequestStorageCloudParameterModel? payload)
    {
        TResponseModel<FoundParameterModel[]?> res = new();

        return Task.FromResult(res);
    }
}