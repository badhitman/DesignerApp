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

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.UserName} {this.GivenName} {this.Surname} {this.Email}";
    }

    /// <inheritdoc/>
    public static UserInfoModel Build(string userId, string? userName, string? email, string? phoneNumber, long? telegramId, bool emailConfirmed, DateTimeOffset? lockoutEnd, bool lockoutEnabled, int accessFailedCount, string? firstName, string? lastName, string[]? roles = null, EntryAltModel[]? claims = null)
        => new()
        {
            GivenName = firstName,
            Surname = lastName,
            UserId = userId,
            Email = email,
            UserName = userName,
            PhoneNumber = phoneNumber,
            TelegramId = telegramId,
            EmailConfirmed = emailConfirmed,
            LockoutEnd = lockoutEnd,
            LockoutEnabled = lockoutEnabled,
            AccessFailedCount = accessFailedCount,
            Roles = roles,
            Claims = claims
        };

    /// <inheritdoc/>
    public static UserInfoModel BuildSystem()
    {
        return new()
        {
            UserId = GlobalStaticConstants.Roles.System,
            Email = GlobalStaticConstants.Roles.System,
            EmailConfirmed = true,
            Roles = [GlobalStaticConstants.Roles.System],
            UserName = "Система",
        };
    }
}