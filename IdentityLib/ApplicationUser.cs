////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace IdentityLib;

/// <summary>
/// Пользователь [Identity]
/// </summary>
[Index(nameof(ChatTelegramId))]
[Index(nameof(NormalizedFirstNameUpper)), Index(nameof(NormalizedLastNameUpper))]
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Telegram Id
    /// </summary>
    public long? ChatTelegramId { get; set; }

    /// <summary>
    /// FirstName
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// NormalizedFirstNameUpper
    /// </summary>
    public string? NormalizedFirstNameUpper { get; set; }

    /// <summary>
    /// LastName
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// NormalizedLastNameUpper
    /// </summary>
    public string? NormalizedLastNameUpper { get; set; }


    /// <inheritdoc/>
    public static explicit operator UserInfoModel(ApplicationUser app_user)
    {
        return UserInfoModel.Build(
            userId: app_user.Id,
            userName: app_user.UserName ?? "",
            email: app_user.Email,
            phoneNumber: app_user.PhoneNumber,
            telegramId: app_user.ChatTelegramId,
            emailConfirmed: app_user.EmailConfirmed,
            lockoutEnd: app_user.LockoutEnd,
            lockoutEnabled: app_user.LockoutEnabled,
            accessFailedCount: app_user.AccessFailedCount,
            firstName: app_user.FirstName,
            lastName: app_user.LastName);
    }
}