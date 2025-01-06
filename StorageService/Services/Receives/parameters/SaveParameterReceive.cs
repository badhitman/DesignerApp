////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// Save parameter
/// </summary>
public class SaveParameterReceive(ISerializeStorage serializeStorageRepo, ILogger<SaveParameterReceive> LoggerRepo)
    : IResponseReceive<StorageCloudParameterPayloadModel?, TResponseModel<int?>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SaveCloudParameterReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>?> ResponseHandleAction(StorageCloudParameterPayloadModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        req.Normalize();
        LoggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        Regex rx = new(@"\s+", RegexOptions.Compiled);
        StorageCloudParameterModelDB store_db = new()
        {
            ApplicationName = rx.Replace(req.ApplicationName.Trim(), " "),
            PropertyName = req.PropertyName,
            SerializedDataJson = req.SerializedDataJson,
            PrefixPropertyName = req.PrefixPropertyName is null ? null : rx.Replace(req.PrefixPropertyName.Trim(), " "),
            OwnerPrimaryKey = req.OwnerPrimaryKey,
            TypeName = req.TypeName,
            CreatedAt = DateTime.UtcNow,
        };

        return await serializeStorageRepo.FlushParameter(store_db, req.TrimHistory);
    }
}