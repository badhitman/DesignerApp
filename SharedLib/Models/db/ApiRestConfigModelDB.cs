////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Настройки api/rest
/// </summary>
public class ApiRestConfigModelDB : EntryModel
{
    /// <summary>
    /// Адрес
    /// </summary>
    public string? AddressBaseUri { get; set; }

    /// <summary>
    /// Токен доступа
    /// </summary>
    public string? TokenAccess { get; set; }

    /// <summary>
    /// Имя заголовка
    /// </summary>
    public string HeaderName { get; set; } = "token-access";

    /// <summary>
    /// Папки синхронизации
    /// </summary>
    public List<SyncDirectoryModelDB>? SyncDirectories { get; set; }
}