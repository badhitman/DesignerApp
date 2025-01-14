////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// Save file
/// </summary>
public class SaveFileReceive(ILogger<SaveFileReceive> LoggerRepo, ISerializeStorage serializeStorageRepo)
    : IResponseReceive<TAuthRequestModel<StorageImageMetadataModel>?, TResponseModel<StorageFileModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SaveFileReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<StorageFileModelDB>?> ResponseHandleAction(TAuthRequestModel<StorageImageMetadataModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        LoggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        return await serializeStorageRepo.SaveFile(req);
    }
}