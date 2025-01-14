////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// TagsSelectReceive
/// </summary>
public class TagsSelectReceive(ILogger<TagsSelectReceive> loggerRepo, ISerializeStorage serializeStorageRepo) 
    : IResponseReceive<TPaginationRequestModel<SelectMetadataRequestModel>?, TPaginationResponseModel<TagModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.TagsSelectReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<TagModelDB>?> ResponseHandleAction(TPaginationRequestModel<SelectMetadataRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        return await serializeStorageRepo.TagsSelect(req);
    }
}