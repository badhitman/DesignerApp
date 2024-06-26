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
//#if DEBUG
//        Debug.WriteLine(JsonConvert.SerializeObject(app_user));
//#endif

        return UserInfoModel.Build(app_user.Id, app_user.Email, app_user.UserName, app_user.PhoneNumber, app_user.TelegramId, app_user.EmailConfirmed, app_user.LockoutEnd, app_user.LockoutEnabled, app_user.AccessFailedCount);
    }
}