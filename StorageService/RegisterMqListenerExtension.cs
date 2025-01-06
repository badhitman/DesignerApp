////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;
using Transmission.Receives.storage;

namespace StorageService;

/// <summary>
/// MQ listen
/// </summary>
public static class RegisterMqListenerExtension
{
    /// <summary>
    /// RegisterMqListeners
    /// </summary>
    public static IServiceCollection StorageRegisterMqListeners(this IServiceCollection services)
    {
        return services
            .RegisterMqListener<SaveParameterReceive, StorageCloudParameterPayloadModel, TResponseModel<int?>>()
            .RegisterMqListener<SaveFileReceive, StorageImageMetadataModel, TResponseModel<StorageFileModelDB>>()
            .RegisterMqListener<TagSetReceive, TagSetModel, TResponseModel<bool>>()
            .RegisterMqListener<SetWebConfigReceive, WebConfigModel, ResponseBaseModel>()
            .RegisterMqListener<ReadFileReceive, TAuthRequestModel<RequestFileReadModel>, TResponseModel<FileContentModel>>()
            .RegisterMqListener<TagsSelectReceive, TPaginationRequestModel<SelectMetadataRequestModel>, TPaginationResponseModel<TagModelDB>>()
            .RegisterMqListener<FilesAreaGetMetadataReceive, FilesAreaMetadataRequestModel, TResponseModel<FilesAreaMetadataModel[]>>()
            .RegisterMqListener<FilesSelectReceive, TPaginationRequestModel<SelectMetadataRequestModel>, TPaginationResponseModel<StorageFileModelDB>>()
            .RegisterMqListener<ReadParameterReceive, StorageMetadataModel, TResponseModel<StorageCloudParameterPayloadModel>>()
            .RegisterMqListener<ReadParametersReceive, StorageMetadataModel[], TResponseModel<List<StorageCloudParameterPayloadModel>>>()
            .RegisterMqListener<FindParametersReceive, RequestStorageBaseModel, TResponseModel<FoundParameterModel[]?>>()
            ;
    }
}