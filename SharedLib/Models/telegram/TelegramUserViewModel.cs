////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// TelegramUserViewModel
/// </summary>
public class TelegramUserViewModel : TelegramUserBaseModel
{
    /// <summary>
    /// Email (identity)
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Id (identity)
    /// </summary>
    public string? UserId { get; set; }

    /// <inheritdoc/>
    public static TelegramUserViewModel Build(TelegramUserModelDb user, string? id, string? email)
    {
        return new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = email,
            UserId = id,
            Id = user.Id,
            IsBot = user.IsBot,
            IsDisabled = user.IsDisabled,
            TelegramId = user.TelegramId,
            Username = user.Username,
            DialogTelegramTypeHandler = user.DialogTelegramTypeHandler,
            MainTelegramMessageId = user.MainTelegramMessageId,
        };
    }
}