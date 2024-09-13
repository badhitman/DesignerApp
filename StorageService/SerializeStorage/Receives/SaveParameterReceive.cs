﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Save parameter
/// </summary>
public class SaveParameterReceive(ISerializeStorage serializeStorageRepo)
    : IResponseReceive<StorageCloudParameterPayloadModel?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SaveCloudParameterReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(StorageCloudParameterPayloadModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        StorageCloudParameterModelDB store_db = new()
        {
            ApplicationName = req.ApplicationName,
            Name = req.Name,
            SerializedDataJson = req.SerializedDataJson,
            PrefixPropertyName = req.PrefixPropertyName,
            OwnerPrimaryKey = req.OwnerPrimaryKey,
            TypeName = req.TypeName,
        };

        return await serializeStorageRepo.FlushParameter(store_db);
    }
}