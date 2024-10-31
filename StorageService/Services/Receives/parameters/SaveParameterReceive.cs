////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using System.Text.RegularExpressions;

namespace Transmission.Receives.storage;

/// <summary>
/// Save parameter
/// </summary>
public class SaveParameterReceive(ISerializeStorage serializeStorageRepo,ILogger<SaveParameterReceive> LoggerRepo)
    : IResponseReceive<StorageCloudParameterPayloadModel?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SaveCloudParameterReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(StorageCloudParameterPayloadModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
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
        };

        return await serializeStorageRepo.FlushParameter(store_db, req.TrimHistory);
    }
}