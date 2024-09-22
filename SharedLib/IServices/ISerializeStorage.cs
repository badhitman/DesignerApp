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
    /// <summary>
    /// Сохранить параметр
    /// </summary>
    /// <typeparam name="T">Тип сохраняемых данных (сериализируемый)</typeparam>
    /// <param name="obj">Данные для сохранения</param>
    /// <param name="set">Метаданные</param>
    /// <param name="trimHistory">Удалить предыдущие значения (очистить историю значений)</param>
    public Task Save<T>(T obj, StorageCloudParameterModel set, bool trimHistory = false);

    /// <summary>
    /// Прочитать значение параметра. null - если значения нет
    /// </summary>
    /// <typeparam name="T">Тип данных (для десериализации из JSON)</typeparam>
    /// <remarks>
    /// Возвращается самое актуальное значение (последнее установленное). Хранится история значений - если значение будет часто меняться будет ротация стека накопленных значений с усечением от 150 до 100.
    /// Проверка переполнения происходит при каждой команде сохранения.
    /// </remarks>
    public Task<T?> Read<T>(StorageCloudParameterModel req);

    /// <summary>
    /// Поиск значений параметров
    /// </summary>
    /// <typeparam name="T">Тип данных (для десериализации из JSON)</typeparam>
    public Task<T?[]> Find<T>(RequestStorageCloudParameterModel req);

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
    public Task<TResponseModel<StorageCloudParameterPayloadModel?>> ReadParameter(StorageCloudParameterModel req);

    /// <summary>
    /// Поиск значений параметров
    /// </summary>
    public Task<TResponseModel<FoundParameterModel[]?>> Find(RequestStorageCloudParameterModel req);
}