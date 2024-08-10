////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Хранимый облачный параметр
/// </summary>
/// <remarks>
/// Приложения могут хранить любые параметры в общем микро-сервисе.
/// Они могут сохранять и извлекать (запрашивать) любые сериализуемые данные
/// </remarks>
public class StorageCloudParameterModel : RequestStorageCloudParameterModel
{
    /// <summary>
    /// Префикс имени (опционально)
    /// </summary>
    public string? PrefixPropertyName { get; set; }

    /// <summary>
    /// Связанный PK строки базы данных (опционально)
    /// </summary>
    public int? OwnerPrimaryKey { get; set; }
}