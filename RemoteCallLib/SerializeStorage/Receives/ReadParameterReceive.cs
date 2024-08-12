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
    : IResponseReceive<StorageCloudParameterModel?, StorageCloudParameterPayloadModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadCloudParameterReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<StorageCloudParameterPayloadModel?>> ResponseHandleAction(StorageCloudParameterModel? request)
    {
        ArgumentNullException.ThrowIfNull(request);

#if DEBUG
        TResponseModel<StorageCloudParameterPayloadModel?> _debug = await serializeStorageRepo.ReadParameter(request);
#endif

        return await serializeStorageRepo.ReadParameter(request);
    }
}