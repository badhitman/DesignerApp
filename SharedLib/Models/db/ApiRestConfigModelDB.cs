////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Настройки api/rest
/// </summary>
public class ApiRestConfigModelDB : EntryModel
{
    /// <summary>
    /// Адрес
    /// </summary>
    [Required]
    public required string AddressBaseUri { get; set; }

    /// <summary>
    /// Токен доступа
    /// </summary>
    [Required]
    public required string TokenAccess { get; set; }

    /// <summary>
    /// Имя заголовка
    /// </summary>
    [Required]
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
    public virtual void Update(ApiRestConfigModelDB other)
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

    /// <inheritdoc/>
    public static new ApiRestConfigModelDB BuildEmpty()
    {
        return new() { AddressBaseUri = string.Empty, Name = string.Empty, TokenAccess = string.Empty };
    }
}