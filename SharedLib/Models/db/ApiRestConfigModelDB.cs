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

/// <summary>
/// Папка синхронизации
/// </summary>
public class SyncDirectoryModelDB : EntryModel
{
    /// <summary>
    /// LocalDirectory
    /// </summary>
    public string? LocalDirectory { get; set; }

    /// <summary>
    /// RemoteDirectory
    /// </summary>
    public string? RemoteDirectory { get; set; }

    /// <summary>
    /// Родитель
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// Родитель
    /// </summary>
    public ApiRestConfigModelDB? Parent { get; set; }
}