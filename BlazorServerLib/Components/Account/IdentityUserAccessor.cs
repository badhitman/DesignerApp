using IdentityLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BlazorWebLib.Components.Account;

/// <summary>
/// Identity User Accessor
/// </summary>
public sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager)
{
    /// <summary>
    /// Get User
    /// </summary>
    public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
    {
        ApplicationUser? user = await userManager.GetUserAsync(context.User);
        return user ?? throw new Exception("user is null. error {6C94885B-C0D3-408E-84D1-A613E43F19A6}");
    }
}