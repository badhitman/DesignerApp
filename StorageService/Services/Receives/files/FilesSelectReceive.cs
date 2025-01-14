////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// FilesSelectReceive
/// </summary>
public class FilesSelectReceive(ILogger<FilesSelectReceive> loggerRepo, ISerializeStorage serializeStorageRepo) 
    : IResponseReceive<TPaginationRequestModel<SelectMetadataRequestModel>?, TPaginationResponseModel<StorageFileModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FilesSelectReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<StorageFileModelDB>?> ResponseHandleAction(TPaginationRequestModel<SelectMetadataRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        return await serializeStorageRepo.FilesSelect(req);
    }
}