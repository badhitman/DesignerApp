////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Identity;
using SharedLib;

namespace IdentityLib;

/// <summary>
/// Add profile data for application users by adding properties to the ApplicationUser class
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Telegram Id
    /// </summary>
    public long? TelegramId { get; set; }

    /// <inheritdoc/>
    public static explicit operator UserInfoModel(ApplicationUser app_user)
    {
        return UserInfoModel.Build(
            userId: app_user.Id,
            userName: app_user.UserName ?? "",
            email: app_user.Email,
            phoneNumber: app_user.PhoneNumber,
            telegramId: app_user.TelegramId,
            emailConfirmed: app_user.EmailConfirmed,
            lockoutEnd: app_user.LockoutEnd,
            lockoutEnabled: app_user.LockoutEnabled,
            accessFailedCount: app_user.AccessFailedCount);
    }
}