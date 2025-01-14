////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// Read file
/// </summary>
public class ReadFileReceive(    ILogger<ReadFileReceive> LoggerRepo, ISerializeStorage serializeStorageRepo) 
    : IResponseReceive<TAuthRequestModel<RequestFileReadModel>?, TResponseModel<FileContentModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadFileReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<FileContentModel>?> ResponseHandleAction(TAuthRequestModel<RequestFileReadModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        LoggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        return await serializeStorageRepo.ReadFile(req);
    }
}