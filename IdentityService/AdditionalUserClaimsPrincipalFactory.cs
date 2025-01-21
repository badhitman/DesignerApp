using IdentityLib;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace IdentityService;

/// <summary>
/// AdditionalUserClaimsPrincipalFactory
/// </summary>
public class AdditionalUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor) :
        UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>(userManager, roleManager, optionsAccessor)
{
    /// <inheritdoc/>
    public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        ClaimsPrincipal principal = await base.CreateAsync(user);
        ClaimsIdentity? identity = (ClaimsIdentity?)principal.Identity;

        List<Claim> claims = [user.TwoFactorEnabled ? new Claim("amr", "mfa") : new Claim("amr", "pwd")];

        identity?.AddClaims(claims);
        return principal;
    }
}
