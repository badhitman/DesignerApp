////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IToolsSystemExtService
/// </summary>
public interface IToolsSystemHTTPRestService : IToolsSystemService
{
    /// <summary>
    /// GetMe
    /// </summary>
    public Task<TResponseModel<ExpressProfileResponseModel>> GetMe();

    /// <summary>
    /// Создать сессию порционной (частями) загрузки файлов
    /// </summary>
    public Task<TResponseModel<PartUploadSessionModel>> PartUploadSessionStart(PartUploadSessionStartRequestModel req);

    /// <summary>
    /// Загрузка порции файла
    /// </summary>
    public Task<ResponseBaseModel> PartUpload(SessionFileRequestModel req);
}