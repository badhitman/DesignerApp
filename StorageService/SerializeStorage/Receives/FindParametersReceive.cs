////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Find parameters
/// </summary>
public class FindParametersReceive(ISerializeStorage serializeStorageRepo)
    : IResponseReceive<RequestStorageCloudParameterModel?, FoundParameterModel[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FindCloudParameterReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<FoundParameterModel[]?>> ResponseHandleAction(RequestStorageCloudParameterModel? request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await serializeStorageRepo.Find(request);
    }
}