////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// CheckTelegramUser
/// </summary>
public class CheckTelegramUserModel : TelegramUserBaseModelDb
{
    /// <summary>
    /// Email пользователя
    /// </summary>
    public string? UserEmail { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating if a user has confirmed their email address.
    /// </summary>
    /// <value>True if the email address has been confirmed, otherwise false.</value>
    public virtual bool EmailConfirmed { get; set; }

    /// <summary>
    /// Gets or sets a telephone number for the user.
    /// </summary>
    public virtual string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating if a user has confirmed their telephone address.
    /// </summary>
    /// <value>True if the telephone number has been confirmed, otherwise false.</value>
    public virtual bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating if two factor authentication is enabled for this user.
    /// </summary>
    /// <value>True if 2fa is enabled, otherwise false.</value>
    public virtual bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// Gets or sets the date and time, in UTC, when any user lockout ends.
    /// </summary>
    /// <remarks>
    /// A value in the past means the user is not locked out.
    /// </remarks>
    public virtual DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating if the user could be locked out.
    /// </summary>
    /// <value>True if the user could be locked out, otherwise false.</value>
    public virtual bool LockoutEnabled { get; set; }

    /// <summary>
    /// Gets or sets the number of failed login attempts for the current user.
    /// </summary>
    public virtual int AccessFailedCount { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()} {UserEmail}".Trim();
    }

    /// <inheritdoc/>
    public static new CheckTelegramUserModel? Build(TelegramUserModelDb tgUserDb)
    {
        return new()
        {
            Name = tgUserDb.Name,
            FirstName = tgUserDb.FirstName,
            LastName = tgUserDb.LastName,
            TelegramId = tgUserDb.TelegramId,
            IsBot = tgUserDb.IsBot,
            Id = tgUserDb.Id,
            CreatedAt = tgUserDb.CreatedAt,
            IsDeleted = tgUserDb.IsDeleted,
            DialogTelegramTypeHandler = tgUserDb.DialogTelegramTypeHandler,
            MainTelegramMessageId = tgUserDb.MainTelegramMessageId,
        };
    }

    /// <inheritdoc/>
    public static new CheckTelegramUserModel? Build(CheckTelegramUserHandleModel tgUserDb)
    {
        return new()
        {
            FirstName = tgUserDb.FirstName,
            LastName = tgUserDb.LastName,
            TelegramId = tgUserDb.TelegramUserId,
            IsBot = tgUserDb.IsBot,
            Name = tgUserDb.Username,
        };
    }

    /// <inheritdoc/>
    public static implicit operator CheckTelegramUserModel(CheckTelegramUserHandleModel user)
    {
        return new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsBot = user.IsBot,
            TelegramId = user.TelegramUserId,
            Name = user.Username,
        };
    }
}