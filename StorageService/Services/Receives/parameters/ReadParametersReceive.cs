////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// Read parameter`s list
/// </summary>
public class ReadParametersReceive(ISerializeStorage serializeStorageRepo, ILogger<ReadParametersReceive> LoggerRepo) : IResponseReceive<StorageMetadataModel[]?, TResponseModel<List<StorageCloudParameterPayloadModel>>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadCloudParametersReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<List<StorageCloudParameterPayloadModel>>?> ResponseHandleAction(StorageMetadataModel[]? request)
    {
        ArgumentNullException.ThrowIfNull(request);
        LoggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(request)}");
        return await serializeStorageRepo.ReadParameters(request);
    }
}