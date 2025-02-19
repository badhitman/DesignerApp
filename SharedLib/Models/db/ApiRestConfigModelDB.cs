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

    /// <summary>
    /// Команды (удалённый запуск)
    /// </summary>
    public List<ExeCommandModelDB>? CommandsRemote { get; set; }

    /// <inheritdoc/>
    public void Reload(ApiRestConfigModelDB other)
    {
        AddressBaseUri = other.AddressBaseUri;
        TokenAccess = other.TokenAccess;
        HeaderName = other.HeaderName;
        Name = other.Name;
        Id = other.Id;

        SyncDirectories = other.SyncDirectories;
        CommandsRemote = other.CommandsRemote;
    }

    /// <inheritdoc/>
    public void Empty()
    {
        AddressBaseUri = string.Empty;
        TokenAccess = string.Empty;
        HeaderName = "token-access";
        Name = string.Empty;
        Id = 0;

        SyncDirectories = [];
        CommandsRemote = [];
    }
}