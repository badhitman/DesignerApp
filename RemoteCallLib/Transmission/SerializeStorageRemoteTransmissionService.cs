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
    public async Task<TResponseModel<List<T>?>> ReadParameters<T>(StorageMetadataModel[] req)
    {
        TResponseModel<List<StorageCloudParameterPayloadModel>> response_payload = await rabbitClient.MqRemoteCall<TResponseModel<List<StorageCloudParameterPayloadModel>>>(GlobalStaticConstants.TransmissionQueues.ReadCloudParametersReceive, req);
        TResponseModel<List<T>?> res = new();
        if (!response_payload.Success())
        {
            res.Messages = response_payload.Messages;
            return res;
        }

        if (response_payload.Response is null || response_payload.Response.Count == 0)
            return res;

        res.Response = response_payload.Response.Select(x => JsonConvert.DeserializeObject<T>(x.SerializedDataJson)).ToList()!;
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<T?>> ReadParameter<T>(StorageMetadataModel req)
    {
        TResponseModel<StorageCloudParameterPayloadModel> response_payload = await rabbitClient.MqRemoteCall<TResponseModel<StorageCloudParameterPayloadModel>>(GlobalStaticConstants.TransmissionQueues.ReadCloudParameterReceive, req);
        TResponseModel<T?> res = new() { Messages = response_payload.Messages };

        if (!response_payload.Success())
        {
            res.Messages = response_payload.Messages;
            return res;
        }

        if (response_payload.Response is null)
            return res;

        res.Response = JsonConvert.DeserializeObject<T>(response_payload.Response.SerializedDataJson);
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<T?[]?>> FindParameters<T>(RequestStorageBaseModel req)
    {
        TResponseModel<FoundParameterModel[]> response_payload = await rabbitClient.MqRemoteCall<TResponseModel<FoundParameterModel[]>>(GlobalStaticConstants.TransmissionQueues.FindCloudParameterReceive, req);
        TResponseModel<T?[]?> res = new();
        if (!response_payload.Success())
        {
            res.Messages = response_payload.Messages;
            return res;
        }

        if (response_payload.Response is null)
            return res;

        res.Response = response_payload
            .Response
            .Select(x => JsonConvert.DeserializeObject<T>(x.SerializedDataJson))
            .ToArray();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> SaveParameter<T>(T payload_query, StorageMetadataModel store, bool trim, bool waitResponse = true)
    {
        if (payload_query is null)
            throw new ArgumentNullException(nameof(payload_query));

        StorageCloudParameterPayloadModel set_req = new()
        {
            TrimHistory = trim,
            ApplicationName = store.ApplicationName,
            PropertyName = store.PropertyName,
            SerializedDataJson = JsonConvert.SerializeObject(payload_query, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings),
            TypeName = payload_query.GetType().FullName!,
            OwnerPrimaryKey = store.OwnerPrimaryKey,
            PrefixPropertyName = store.PrefixPropertyName,
        };

        return await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.SaveCloudParameterReceive, set_req, waitResponse);
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<StorageFileModelDB>> SaveFile(StorageImageMetadataModel? req)
        => await rabbitClient.MqRemoteCall<TResponseModel<StorageFileModelDB>>(GlobalStaticConstants.TransmissionQueues.SaveFileReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<FileContentModel>> ReadFile(TAuthRequestModel<RequestFileReadModel>? req)
        => await rabbitClient.MqRemoteCall<TResponseModel<FileContentModel>>(GlobalStaticConstants.TransmissionQueues.ReadFileReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<StorageFileModelDB>>> FilesSelect(TPaginationRequestModel<SelectMetadataRequestModel> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<TPaginationResponseModel<StorageFileModelDB>>>(GlobalStaticConstants.TransmissionQueues.FilesSelectReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<FilesAreaMetadataModel[]>> FilesAreaGetMetadata(FilesAreaMetadataRequestModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<FilesAreaMetadataModel[]>>(GlobalStaticConstants.TransmissionQueues.FilesAreaGetMetadataReceive, req);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<TagModelDB>> TagsSelect(TPaginationRequestModel<SelectMetadataRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<TagModelDB>>(GlobalStaticConstants.TransmissionQueues.TagsSelectReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> TagSet(TagSetModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.TagSetReceive, req);
}