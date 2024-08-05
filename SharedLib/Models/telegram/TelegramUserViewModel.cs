////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// TelegramUserViewModel
/// </summary>
public class TelegramUserViewModel : TelegramUserBaseModelDb
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
            CreatedAt = user.CreatedAt,
            Email = email,
            UserId = id,
            Id = user.Id,
            IsBot = user.IsBot,
            IsDisabled = user.IsDisabled,
            TelegramId = user.TelegramId,
            Name = user.Name,
            DialogTelegramTypeHandler = user.DialogTelegramTypeHandler,
            MainTelegramMessageId = user.MainTelegramMessageId,
        };
    }
}