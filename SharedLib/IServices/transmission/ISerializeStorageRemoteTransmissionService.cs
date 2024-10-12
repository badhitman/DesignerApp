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
    /// ReadFile
    /// </summary>
    public Task<TResponseModel<StorageFileResponseModel>> ReadFile(int? req);

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
    /// Сохранить параметр
    /// </summary>
    public Task<TResponseModel<int>> SaveParameter<T>(T payload_query, StorageMetadataModel store, bool trim);

    /// <summary>
    /// Найти/подобрать значения параметров (со всей историей значений)
    /// </summary>
    public Task<TResponseModel<T?[]?>> FindParameters<T>(RequestStorageBaseModel req);
}