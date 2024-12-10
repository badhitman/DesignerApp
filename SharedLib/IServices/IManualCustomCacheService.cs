////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Интерфейс Cache
/// </summary>
public interface IManualCustomCacheService
{
    #region get
    /// <summary>
    /// Прочитать из Cache данные в виде строки
    /// </summary>
    /// <param name="mem_key">Комплексный/полный ключ доступа Cache данным</param>
    /// <param name="token"></param>
    /// <returns>Данные, прочитанные из Cache по комплексному/полному имени ключа</returns>
    public Task<string?> GetStringValueAsync(MemCacheComplexKeyModel mem_key, CancellationToken token = default);

    /// <summary>
    /// Прочитать (асинхронно) из Cache данные в виде строки
    /// </summary>
    /// <param name="pref">Префикс ключа доступа к данным</param>
    /// <param name="id">Имя/идентификатор (конечный) данных</param>
    /// <param name="token"></param>
    /// <returns>Данные, прочитанные из Cache</returns>
    public Task<string?> GetStringValueAsync(MemCachePrefixModel pref, string id = "", CancellationToken token = default);

    /// <summary>
    /// Прочитать из Cache данные в виде строки
    /// </summary>
    /// <returns>Данные, прочитанные из Cache по комплексному/полному имени ключа</returns>
    public Task<T?> GetObjectAsync<T>(string mem_key, CancellationToken token = default);

    /// <summary>
    /// Прочитать из Cache данные в виде строки
    /// </summary>
    /// <param name="mem_key">Комплексный/полный ключ доступа Cache данным</param>
    /// <param name="token"></param>
    /// <returns>Данные, прочитанные из Cache по комплексному/полному имени ключа</returns>
    public Task<T?> GetObjectAsync<T>(MemCacheComplexKeyModel mem_key, CancellationToken token = default);

    /// <summary>
    /// Прочитать (асинхронно) из Cache данные в виде строки
    /// </summary>
    /// <param name="pref">Префикс ключа доступа к данным</param>
    /// <param name="id">Имя/идентификатор (конечный) данных</param>
    /// <param name="token"></param>
    /// <returns>Данные, прочитанные из Cache</returns>
    public Task<T?> GetObjectAsync<T>(MemCachePrefixModel pref, string id = "", CancellationToken token = default);


    /// <summary>
    /// Прочитать из Cache данные в виде byte[]
    /// </summary>
    /// <returns>Данные, прочитанные из Cache по комплексному/полному имени ключа</returns>
    public Task<byte[]?> GetBytesAsync(string mem_key, CancellationToken token = default);

    /// <summary>
    /// Прочитать из Cache данные в виде byte[]
    /// </summary>
    /// <param name="mem_key">Комплексный/полный ключ доступа Cache данным</param>
    /// <param name="token"></param>
    /// <returns>Данные, прочитанные из Cache по комплексному/полному имени ключа</returns>
    public Task<byte[]?> GetBytesAsync(MemCacheComplexKeyModel mem_key, CancellationToken token = default);

    /// <summary>
    /// Прочитать (асинхронно) из Cache данные в виде byte[]
    /// </summary>
    /// <param name="pref">Префикс ключа доступа к данным</param>
    /// <param name="id">Имя/идентификатор (конечный) данных</param>
    /// <param name="token"></param>
    /// <returns>Данные, прочитанные из Cache</returns>
    public Task<byte[]?> GetBytesAsync(MemCachePrefixModel pref, string id = "", CancellationToken token = default);
    #endregion

    #region set/upd
    /// <summary>
    /// Обновить/записать (асинхронно) данные в Cache
    /// </summary>
    /// <param name="key">Ключ/указатель на данные в Cache</param>
    /// <param name="valueData">Значение для записи в Cache</param>
    /// <param name="expiry">Срок годности/хранения данных в Cache (null - по умолчанию = бессрочно)</param>
    /// <param name="token"></param>
    public Task WriteBytesAsync(MemCacheComplexKeyModel key, byte[] valueData, TimeSpan? expiry = null, CancellationToken token = default);

    /// <summary>
    /// Обновить/записать (асинхронно) данные в Cache
    /// </summary>
    /// <param name="key">Ключ/указатель на данные в Cache</param>
    /// <param name="value">Значение для записи в Cache</param>
    /// <param name="expiry">Срок годности/хранения данных в Cache (null - по умолчанию = бессрочно)</param>
    /// <param name="token"></param>
    /// <returns>Результат операции</returns>
    public Task<bool> SetStringAsync(MemCacheComplexKeyModel key, string value, TimeSpan? expiry = null, CancellationToken token = default);

    /// <summary>
    /// Обновить/записать данные в Cache
    /// </summary>
    /// <param name="pref">Префикс ключа доступа к данным Cache</param>
    /// <param name="id">Имя/идентификатор (конечный) доступа к данным Cache</param>
    /// <param name="value">Значение для записи в Cache</param>
    /// <param name="expiry">Срок годности/хранения данных в Cache (null - по умолчанию = бессрочно)</param>
    /// <param name="token"></param>
    /// <returns>Результат операции</returns>
    public Task<bool> SetStringAsync(MemCachePrefixModel pref, string id, string value, TimeSpan? expiry = null, CancellationToken token = default);

    /// <summary>
    /// Обновить/записать (асинхронно) данные в Cache
    /// </summary>
    /// <param name="key">Ключ/указатель на данные в Cache</param>
    /// <param name="value">Значение для записи в Cache</param>
    /// <param name="expiry">Срок годности/хранения данных в Cache (null - по умолчанию = бессрочно)</param>
    /// <param name="token"></param>
    /// <returns>Результат операции</returns>
    public Task SetObjectAsync<T>(MemCacheComplexKeyModel key, T value, TimeSpan? expiry = null, CancellationToken token = default);

    /// <summary>
    /// Обновить/записать данные в Cache
    /// </summary>
    /// <param name="pref">Префикс ключа доступа к данным Cache</param>
    /// <param name="id">Имя/идентификатор (конечный) доступа к данным Cache</param>
    /// <param name="value">Значение для записи в Cache</param>
    /// <param name="expiry">Срок годности/хранения данных в Cache (null - по умолчанию = бессрочно)</param>
    /// <param name="token"></param>
    /// <returns>Результат операции</returns>
    public Task SetObjectAsync<T>(MemCachePrefixModel pref, string id, T value, TimeSpan? expiry = null, CancellationToken token = default);

    /// <summary>
    /// Обновить/записать данные в Cache
    /// </summary>
    /// <param name="mem_key">Имя/идентификатор (конечный) доступа к данным Cache</param>
    /// <param name="value">Значение для записи в Cache</param>
    /// <param name="expiry">Срок годности/хранения данных в Cache (null - по умолчанию = бессрочно)</param>
    /// <param name="token"></param>
    /// <returns>Результат операции</returns>
    public Task SetObjectAsync<T>(string mem_key, T value, TimeSpan? expiry = null, CancellationToken token = default);
    #endregion

    #region remove
    /// <summary>
    /// Удалить (асинхронно) данные из Cache
    /// </summary>
    /// <param name="key">Ключ/указатель на данные в Cache</param>
    /// <returns>Результат операции</returns>
    public Task<bool> RemoveAsync(string key);

    /// <summary>
    /// Удалить (асинхронно) данные из Cache
    /// </summary>
    /// <param name="key">Ключ/указатель на данные в Cache</param>
    /// <returns>Результат операции</returns>
    public Task<bool> RemoveAsync(MemCacheComplexKeyModel key);

    /// <summary>
    /// Удалить (асинхронно) данные из Cache
    /// </summary>
    /// <param name="pref">Префикс ключа доступа к данным Cache</param>
    /// <param name="id">Имя/идентификатор (конечный) доступа к данным Cache</param>
    /// <returns>Результат операции</returns>
    public Task<bool> RemoveAsync(MemCachePrefixModel pref, string id);
    #endregion
}