////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// Read parameter
/// </summary>
public class ReadParameterReceive(ISerializeStorage serializeStorageRepo)
    : IResponseReceive<StorageMetadataModel?, StorageCloudParameterPayloadModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadCloudParameterReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<StorageCloudParameterPayloadModel?>> ResponseHandleAction(StorageMetadataModel? request)
    {
        ArgumentNullException.ThrowIfNull(request);

//#if DEBUG
//        TResponseModel<StorageCloudParameterPayloadModel?> _debug = await serializeStorageRepo.ReadParameter(request);
//#endif

        return await serializeStorageRepo.ReadParameter(request);
    }
}