////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace IdentityLib;

/// <summary>
/// Add profile data for application users by adding properties to the ApplicationUser class
/// </summary>
[Index(nameof(ChatTelegramId))]
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
    /// LastName
    /// </summary>
    public string? LastName { get; set; }

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