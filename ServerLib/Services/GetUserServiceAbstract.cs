////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using IdentityLib;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedLib;
using System.Security.Claims;

namespace ServerLib;

/// <summary>
/// GetUserService
/// </summary>
public abstract class GetUserServiceAbstract(IHttpContextAccessor httpContextAccessor, IDbContextFactory<IdentityAppDbContext> identityDbFactory)
{
    /// <summary>
    /// Read Identity user data.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public async Task<ApplicationUserResponseModel> GetUser(string? userId = null)
    {
        ApplicationUser? user;
        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();
        string msg;
        if (string.IsNullOrWhiteSpace(userId))
        {
            string? user_id = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (user_id is null)
            {
                msg = "HttpContext is null (текущий пользователь) не авторизован. info D485BA3C-081C-4E2F-954D-759A181DCE78";
                return new ApplicationUserResponseModel() { Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Info, Text = msg }] };
            }
            else
            {
                user = await identityContext.Users.FirstOrDefaultAsync(x => x.Id == user_id);

                //#if DEBUG
                //                Debug.WriteLine(JsonConvert.SerializeObject(user));
                //                Debug.WriteLine(JsonConvert.SerializeObject(await userManager.FindByIdAsync(user.Id)));
                //#endif

                return new ApplicationUserResponseModel()
                {
                    ApplicationUser = user
                };
            }
        }
        user = await identityContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            msg = $"Identity user ({nameof(userId)}: `{userId}`) не найден. error {{9D6C3816-7A39-424F-8EF1-B86732D46BD7}}";
            return (ApplicationUserResponseModel)ResponseBaseModel.CreateError(msg);
        }
        return new ApplicationUserResponseModel()
        {
            ApplicationUser = user
        };
    }
}