////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IClientHTTPRestService
/// </summary>
public interface IClientHTTPRestService : IServerToolsService
{
    /// <summary>
    /// GetMe
    /// </summary>
    public Task<TResponseModel<ExpressProfileResponseModel>> GetMe(CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать сессию порционной (частями) загрузки файлов
    /// </summary>
    public Task<TResponseModel<PartUploadSessionModel>> PartUploadSessionStart(PartUploadSessionStartRequestModel req);

    /// <summary>
    /// Загрузка порции файла
    /// </summary>
    public Task<ResponseBaseModel> PartUpload(SessionFileRequestModel req);
}