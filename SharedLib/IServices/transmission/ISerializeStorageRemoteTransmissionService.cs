////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Serialize Remote Transmission Service
/// </summary>
public interface ISerializeStorageRemoteTransmissionService
{
    /// <summary>
    /// TagSet
    /// </summary>
    public Task<TResponseModel<bool>> TagSet(TagSetModel req);

    /// <summary>
    /// TagsSelect
    /// </summary>
    public Task<TPaginationResponseModel<TagModelDB>> TagsSelect(TPaginationRequestModel<SelectMetadataRequestModel> req);

    /// <summary>
    /// Получить сводку (метаданные) по пространствам хранилища
    /// </summary>
    /// <remarks>
    /// Общий размер и количество группируется по AppName
    /// </remarks>
    public Task<TResponseModel<FilesAreaMetadataModel[]>> FilesAreaGetMetadata(FilesAreaMetadataRequestModel req);

    /// <summary>
    /// Files select
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<StorageFileModelDB>>> FilesSelect(TPaginationRequestModel<SelectMetadataRequestModel> req);

    /// <summary>
    /// ReadFile
    /// </summary>
    public Task<TResponseModel<FileContentModel>> ReadFile(TAuthRequestModel<RequestFileReadModel>? req);

    /// <summary>
    /// Сохранить файл
    /// </summary>
    public Task<TResponseModel<StorageFileModelDB>> SaveFile(StorageImageMetadataModel? req);

    /// <summary>
    /// Прочитать значение параметра. null - если значения нет
    /// </summary>
    /// <typeparam name="T">Тип данных (для десериализации из JSON)</typeparam>
    /// <remarks>
    /// Возвращается самое актуальное значение (последнее установленное). Хранится история значений - если значение будет часто меняться будет ротация стека накопленных значений с усечением от 150 до 100.
    /// Проверка переполнения происходит при каждой команде сохранения.
    /// </remarks>
    public Task<TResponseModel<T?>> ReadParameter<T>(StorageMetadataModel req);

    /// <summary>
    /// Прочитать значения параметров. Данные запрашиваемых параметров, которые отсутствуют в БД - не попадут в возвращаемый ответ.
    /// </summary>
    /// <typeparam name="T">Тип данных (для десериализации из JSON)</typeparam>
    /// <remarks>
    /// Возвращается самое актуальное значение (последнее установленное). Хранится история значений - если значение будет часто меняться будет ротация стека накопленных значений с усечением от 150 до 100.
    /// Проверка переполнения происходит при каждой команде сохранения.
    /// </remarks>
    public Task<TResponseModel<List<T>?>> ReadParameters<T>(StorageMetadataModel[] req);

    /// <summary>
    /// Сохранить параметр
    /// </summary>
    public Task<TResponseModel<int>> SaveParameter<T>(T payload_query, StorageMetadataModel store, bool trim, bool waitResponse = true);

    /// <summary>
    /// Найти/подобрать значения параметров (со всей историей значений)
    /// </summary>
    public Task<TResponseModel<T?[]?>> FindParameters<T>(RequestStorageBaseModel req);
}