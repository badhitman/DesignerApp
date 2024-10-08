﻿////////////////////////////////////////////////
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

    /// <inheritdoc/>
    public static TelegramUserViewModel Build(TelegramUserModelDb user, string user_identity_id, string? email)
    {
        return new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = email,
            UserIdentityId = user_identity_id,
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