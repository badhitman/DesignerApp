////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Telegram пользователь
/// </summary>
[Index(nameof(NormalizedName)), Index(nameof(NormalizedFirstName)), Index(nameof(NormalizedLastName))]
public class TelegramUserModelDb : TelegramUserBaseModel
{
    /// <summary>
    /// Username
    /// </summary>
    public string? NormalizedName { get; set; }

    /// <summary>
    /// User's or bot’s first name
    /// </summary>
    public required string NormalizedFirstName { get; set; }

    /// <summary>
    /// Optional. User's or bot’s last name
    /// </summary>
    public string? NormalizedLastName { get; set; }

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
    public static new TelegramUserModelDb Build(CheckTelegramUserHandleModel user)
        => new()
        {
            FirstName = user.FirstName,
            NormalizedFirstName = user.FirstName.ToUpper(),
            LastName = user.LastName,
            NormalizedLastName = user.LastName?.ToUpper(),
            TelegramId = user.TelegramUserId,
            IsBot = user.IsBot,
            Username = user.Username ?? "",
            NormalizedName = user.Username?.ToUpper(),
        };
}