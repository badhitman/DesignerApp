////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Пользователь
/// </summary>
public class UserInfoModel : UserInfoMainModel
{
    /// <inheritdoc/>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Пользователь привязал Telegram к учётной записи
    /// </summary>
    public long? TelegramId { get; set; }

    /// <summary>
    /// Флаг, указывающий, подтвердил ли пользователь свой адрес электронной почты.
    /// </summary>
    /// <value>True, если адрес электронной почты был подтвержден, в противном случае — false.</value>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// Получает или задает дату и время в формате UTC окончания блокировки пользователя.
    /// </summary>
    /// <remarks>
    /// Значение в прошлом означает, что пользователь не заблокирован.
    /// </remarks>
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// Флаг, указывающий, может ли пользователь быть заблокирован.
    /// </summary>
    public bool LockoutEnabled { get; set; }

    /// <summary>
    /// Получает или задает количество неудачных попыток входа в систему для текущего пользователя.
    /// </summary>
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// Наличие роли admin
    /// </summary>
    public bool IsAdmin => Roles?.Any(x => x.Equals("admin", StringComparison.OrdinalIgnoreCase)) == true;

    /// <inheritdoc/>
    public static UserInfoModel Build(string userId, string? userName, string? email, string? phoneNumber, long? telegramId, bool emailConfirmed, DateTimeOffset? lockoutEnd, bool lockoutEnabled, int accessFailedCount, string[]? roles = null)
        => new()
        {
            UserId = userId,
            Email = email,
            UserName = userName,
            PhoneNumber = phoneNumber,
            TelegramId = telegramId,
            EmailConfirmed = emailConfirmed,
            LockoutEnd = lockoutEnd,
            LockoutEnabled = lockoutEnabled,
            AccessFailedCount = accessFailedCount,
            Roles = roles
        };
}