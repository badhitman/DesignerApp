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
            .RegisterMqListener<SaveParameterReceive, StorageCloudParameterPayloadModel?, int?>()
            .RegisterMqListener<SaveFileReceive, StorageImageMetadataModel?, StorageFileModelDB?>()
            .RegisterMqListener<TagSetReceive, TagSetModel?, bool?>()
            .RegisterMqListener<SetWebConfigReceive, WebConfigModel?, object?>()
            .RegisterMqListener<ReadFileReceive, TAuthRequestModel<RequestFileReadModel>?, FileContentModel?>()
            .RegisterMqListener<TagsSelectReceive, TPaginationRequestModel<SelectMetadataRequestModel>?, TPaginationResponseModel<TagModelDB>?>()
            .RegisterMqListener<FilesAreaGetMetadataReceive, FilesAreaMetadataRequestModel?, FilesAreaMetadataModel[]?>()
            .RegisterMqListener<FilesSelectReceive, TPaginationRequestModel<SelectMetadataRequestModel>?, TPaginationResponseModel<StorageFileModelDB>?>()
            .RegisterMqListener<ReadParameterReceive, StorageMetadataModel?, StorageCloudParameterPayloadModel?>()
            .RegisterMqListener<ReadParametersReceive, StorageMetadataModel[]?, List<StorageCloudParameterPayloadModel>?>()
            .RegisterMqListener<FindParametersReceive, RequestStorageBaseModel?, FoundParameterModel[]?>()
            ;
    }
}