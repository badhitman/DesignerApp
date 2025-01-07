////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// Read parameter
/// </summary>
public class ReadParameterReceive(ISerializeStorage serializeStorageRepo, ILogger<ReadParameterReceive> LoggerRepo) : IResponseReceive<StorageMetadataModel?, TResponseModel<StorageCloudParameterPayloadModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadCloudParameterReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<StorageCloudParameterPayloadModel>?> ResponseHandleAction(StorageMetadataModel? request)
    {
        ArgumentNullException.ThrowIfNull(request);
        LoggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(request)}");

        return await serializeStorageRepo.ReadParameter(request);
    }
}