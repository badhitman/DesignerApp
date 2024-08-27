////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Добавьте свойства в этот класс и обновите AuthenticationStateProviders сервера и клиента, 
/// чтобы предоставить клиенту дополнительную информацию о прошедшем проверку подлинности пользователе.
/// </summary>
public class UserInfoMainModel
{
    /// <summary>
    /// FirstName
    /// </summary>
    public string? GivenName { get; set; }

    /// <summary>
    /// LastName
    /// </summary>
    public string? Surname { get; set; }

    /// <inheritdoc/>
    public required string UserId { get; set; }

    /// <inheritdoc/>
    public string? UserName { get; set; }

    /// <inheritdoc/>
    public string? Email { get; set; }

    /// <summary>
    /// Пользователь привязал Telegram к учётной записи
    /// </summary>
    public long? TelegramId { get; set; }

    /// <summary>
    /// Роли пользователя
    /// </summary>
    public string[]? Roles { get; set; }

    /// <summary>
    /// Claims
    /// </summary>
    public EntryAltModel[]? Claims { get; set; }

    /// <summary>
    /// Роли пользователя в виде одной строки
    /// </summary>
    public string RolesAsString(string separator) => Roles is null || Roles.Length == 0 ? string.Empty : $"{string.Join(separator, Roles.Select(i => $"[{i}]"))}{separator}".Trim();

    /// <summary>
    /// Claims
    /// </summary>
    public string ClaimsAsString(string separator) => Claims is null || Claims.Length == 0 ? string.Empty : $"{string.Join(separator, Claims.Select(y => $"[{y.Id}: {y.Name}]"))}{separator}".Trim();

    /// <summary>
    /// Наличие роли admin
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public bool IsAdmin => Roles?.Any(x => x.Equals(GlobalStaticConstants.Roles.Admin, StringComparison.OrdinalIgnoreCase)) == true;
}