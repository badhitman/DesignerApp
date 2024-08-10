////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// Serialize Storage Remote Transmission Service
/// </summary>
public class SerializeStorageRemoteTransmissionService(IRabbitClient rabbitClient) : ISerializeStorageRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<T?>> ReadParameter<T>(StorageCloudParameterModel req)
    {
        TResponseModel<StorageCloudParameterPayloadModel?> response_payload = await rabbitClient.MqRemoteCall<StorageCloudParameterPayloadModel?>(GlobalStaticConstants.TransmissionQueues.ReadCloudParameterReceive, req);
        TResponseModel<T?> res = new();
        if (!response_payload.Success())
        {
            res.Messages = response_payload.Messages;
            return res;
        }

        if (response_payload.Response is null)
        {
            res.AddError("Получен пустой ответ");
            return res;
        }

        res.Response = JsonConvert.DeserializeObject<T>(response_payload.Response.SerializedDataJson);
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<T?[]?>> FindParameters<T>(RequestStorageCloudParameterModel req)
    {
        TResponseModel<FoundParameterModel[]?> response_payload = await rabbitClient.MqRemoteCall<FoundParameterModel[]?>(GlobalStaticConstants.TransmissionQueues.FindCloudParameterReceive, req);
        TResponseModel<T?[]?> res = new();
        if (!response_payload.Success())
        {
            res.Messages = response_payload.Messages;
            return res;
        }

        if (response_payload.Response is null)
        {
            res.AddError("Получен пустой ответ");
            return res;
        }

        res.Response = response_payload
            .Response
            .Select(x => JsonConvert.DeserializeObject<T>(x.SerializedDataJson))
            .ToArray();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> SaveParameter<T>(T payload_query, StorageCloudParameterModel store)
    {
        if (payload_query is null)
            throw new ArgumentNullException(nameof(payload_query));

        StorageCloudParameterPayloadModel set_req = new()
        {
            ApplicationName = store.ApplicationName,
            Name = store.Name,
            SerializedDataJson = JsonConvert.SerializeObject(payload_query),
            TypeName = payload_query.GetType().FullName!,
            OwnerPrimaryKey = store.OwnerPrimaryKey,
            PrefixPropertyName = store.PrefixPropertyName,
        };

        return await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.SaveCloudParameterReceive, JsonConvert.SerializeObject(set_req));
    }
}