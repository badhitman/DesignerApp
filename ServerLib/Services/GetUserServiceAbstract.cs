////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using IdentityLib;
using SharedLib;
using Microsoft.Extensions.Logging;

namespace ServerLib;

/// <summary>
/// GetUserService
/// </summary>
public abstract class GetUserServiceAbstract(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, ILogger<GetUserServiceAbstract> LoggerRepo)
{
    /// <summary>
    /// Read Identity user data.
    /// Если <paramref name="userId"/> не указан, то команда выполняется для текущего пользователя (запрос/сессия)
    /// </summary>
    public async Task<ApplicationUserResponseModel> GetUser(string? userId = null)
    {
        ApplicationUser? user;

        string msg;
        if (string.IsNullOrWhiteSpace(userId))
        {
            LoggerRepo.LogInformation($"IsAuthenticated:{httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated}");
            LoggerRepo.LogInformation($"Name:{httpContextAccessor.HttpContext?.User.Identity?.Name}");
            if (httpContextAccessor.HttpContext is not null)
                LoggerRepo.LogInformation($"Claims:{string.Join(",", httpContextAccessor.HttpContext.User.Claims.Select(x => $"[{x.ValueType}:{x.Value}]"))}");

            string? user_id = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user_id is null)
            {
                msg = "HttpContext is null (текущий пользователь) не авторизован. info D485BA3C-081C-4E2F-954D-759A181DCE78";
                return new() { Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Info, Text = msg }] };
            }
            else
            {
                user = await userManager.FindByIdAsync(user_id);
                return new()
                {
                    ApplicationUser = user
                };
            }
        }
        user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            msg = $"Identity user ({nameof(userId)}: `{userId}`) не найден. error {{9D6C3816-7A39-424F-8EF1-B86732D46BD7}}";
            return (ApplicationUserResponseModel)ResponseBaseModel.CreateError(msg);
        }
        return new()
        {
            ApplicationUser = user
        };
    }
}