////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Read parameter
/// </summary>
public class ReadParameterReceive(ISerializeStorage serializeStorageRepo)
    : IResponseReceive<StorageCloudParameterReadModel?, StorageCloudParameterPayloadModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadCloudParameterReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<StorageCloudParameterPayloadModel?>> ResponseHandleAction(StorageCloudParameterReadModel? request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await serializeStorageRepo.ReadParameter(request);
    }
}