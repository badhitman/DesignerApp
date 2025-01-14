////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Хранилище параметров приложений
/// </summary>
/// <remarks>
/// Значения/данные сериализуются в JSON строку при сохранении и десерализируются при чтении
/// </remarks>
public interface ISerializeStorage
{
    #region tags
    /// <summary>
    /// FilesAreaGetMetadata
    /// </summary>
    public Task<TResponseModel<FilesAreaMetadataModel[]>> FilesAreaGetMetadata(FilesAreaMetadataRequestModel req);

    /// <summary>
    /// FilesSelect
    /// </summary>
    public Task<TPaginationResponseModel<StorageFileModelDB>> FilesSelect(TPaginationRequestModel<SelectMetadataRequestModel> req);

    /// <summary>
    /// ReadFile
    /// </summary>
    public Task<TResponseModel<FileContentModel>> ReadFile(TAuthRequestModel<RequestFileReadModel> req);

    /// <summary>
    /// SaveFile
    /// </summary>
    public Task<TResponseModel<StorageFileModelDB>> SaveFile(TAuthRequestModel<StorageImageMetadataModel> req);

    /// <summary>
    /// TagSet
    /// </summary>
    public Task<ResponseBaseModel> TagSet(TagSetModel req);

    /// <summary>
    /// TagsSelect
    /// </summary>
    public Task<TPaginationResponseModel<TagModelDB>> TagsSelect(TPaginationRequestModel<SelectMetadataRequestModel> req);
    #endregion

    #region storage parameters
    /// <summary>
    /// Сохранить параметр
    /// </summary>
    /// <typeparam name="T">Тип сохраняемых данных (сериализируемый)</typeparam>
    /// <param name="obj">Данные для сохранения</param>
    /// <param name="set">Метаданные</param>
    /// <param name="trimHistory">Удалить предыдущие значения (очистить историю значений)</param>
    public Task Save<T>(T obj, StorageMetadataModel set, bool trimHistory = false);

    /// <summary>
    /// Прочитать значение параметра. null - если значения нет
    /// </summary>
    /// <typeparam name="T">Тип данных (для десериализации из JSON)</typeparam>
    /// <remarks>
    /// Возвращается самое актуальное значение (последнее установленное). Хранится история значений - если значение будет часто меняться будет ротация стека накопленных значений с усечением от 150 до 100.
    /// Проверка переполнения происходит при каждой команде сохранения.
    /// </remarks>
    public Task<T?> Read<T>(StorageMetadataModel req);

    /// <summary>
    /// Поиск значений параметров
    /// </summary>
    /// <typeparam name="T">Тип данных (для десериализации из JSON)</typeparam>
    public Task<T?[]> Find<T>(RequestStorageBaseModel req);

    /// <summary>
    /// FlushParameter
    /// </summary>
    public Task<TResponseModel<int?>> FlushParameter(StorageCloudParameterModelDB storage, bool trimHistory = false);

    /// <summary>
    /// Прочитать значение параметра. null - если значения нет
    /// </summary>
    /// <remarks>
    /// Возвращается самое актуальное значение (последнее установленное). Хранится история значений - если значение будет часто меняться будет ротация стека накопленных значений с усечением от 150 до 100.
    /// Проверка переполнения происходит при каждой команде сохранения.
    /// </remarks>
    public Task<TResponseModel<StorageCloudParameterPayloadModel>> ReadParameter(StorageMetadataModel req);

    /// <summary>
    /// Прочитать значения параметров. Данные запрашиваемых параметров, которые отсутствуют в БД - не попадут в возвращаемый ответ.
    /// </summary>
    /// <remarks>
    /// Возвращается самое актуальные значения (последнее установленное). Хранится история значений - если значение будет часто меняться будет ротация стека накопленных значений с усечением от 150 до 100.
    /// Проверка переполнения происходит при каждой команде сохранения.
    /// </remarks>
    public Task<TResponseModel<List<StorageCloudParameterPayloadModel>>> ReadParameters(StorageMetadataModel[] req);

    /// <summary>
    /// Поиск значений параметров
    /// </summary>
    public Task<TResponseModel<FoundParameterModel[]?>> Find(RequestStorageBaseModel req);
    #endregion
}