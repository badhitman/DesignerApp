////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Telegram пользователь
/// </summary>
[Index(nameof(NormalizedUserNameUpper)), Index(nameof(NormalizedFirstNameUpper)), Index(nameof(NormalizedLastNameUpper))]
public class TelegramUserModelDb : TelegramUserBaseModel
{
    /// <summary>
    /// Username
    /// </summary>
    public string? NormalizedUserNameUpper { get; set; }

    /// <summary>
    /// User's or bot’s first name
    /// </summary>
    public required string NormalizedFirstNameUpper { get; set; }

    /// <summary>
    /// Optional. User's or bot’s last name
    /// </summary>
    public string? NormalizedLastNameUpper { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        string res = FirstName;
        if (!string.IsNullOrWhiteSpace(LastName))
            res += $" {LastName}";
        if (!string.IsNullOrWhiteSpace(Username))
            res += $" @{Username}";
        return res;
    }

    /// <inheritdoc/>
    public static TelegramUserModelDb Build(CheckTelegramUserHandleModel user, string user_identity_id)
        => new()
        {
            FirstName = user.FirstName,
            NormalizedFirstNameUpper = user.FirstName.ToUpper(),
            LastName = user.LastName,
            NormalizedLastNameUpper = user.LastName?.ToUpper(),
            TelegramId = user.TelegramUserId,
            IsBot = user.IsBot,
            Username = user.Username ?? "",
            NormalizedUserNameUpper = user.Username?.ToUpper(),
            UserIdentityId = user_identity_id,
        };
}