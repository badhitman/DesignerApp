////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using System.Security.Claims;
using System.Net.Mail;
using IdentityLib;
using System.Text;
using SharedLib;

namespace ServerLib;
#pragma warning disable CS9107
/// <summary>
/// Сервис работы с профилями пользователей
/// </summary>
public class UsersProfilesService(
    IEmailSender<ApplicationUser> emailSender,
    UserManager<ApplicationUser> userManager,
    IIdentityTransmission IdentityRepo,
    SignInManager<ApplicationUser> signInManager,
    IUserStore<ApplicationUser> userStore,
    IHttpContextAccessor httpContextAccessor,
    ILogger<UsersProfilesService> LoggerRepo) : IUsersProfilesService
{
#pragma warning restore CS9107
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetPhoneNumberAsync(string? phoneNumber, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        await userManager.SetPhoneNumberAsync(user.ApplicationUser, phoneNumber);
        return ResponseBaseModel.CreateSuccess("Телефон успешно установлен");
    }

    /// <inheritdoc/>
    public async Task<UserBooleanResponseModel> CheckUserPasswordAsync(string password, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        string msg;
        if (!await userManager.CheckPasswordAsync(user.ApplicationUser, password))
        {
            msg = "Ошибка: Неправильный пароль. error {91A2600D-5EBF-4F79-83BE-28F6FA55301C}";
            LoggerRepo.LogError(msg);
            return (UserBooleanResponseModel)ResponseBaseModel.CreateError(msg);
        }

        TResponseModel<UserInfoModel[]> rest = await IdentityRepo.GetUsersIdentity([user.ApplicationUser.Id]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        return new() { UserInfo = rest.Response[0], Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Success, Text = "Пароль проверку прошёл!" }] };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteUserDataAsync(string password, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        bool user_has_pass = await userManager.HasPasswordAsync(user.ApplicationUser);

        if (!user_has_pass || !await userManager.CheckPasswordAsync(user.ApplicationUser, password))
            return ResponseBaseModel.CreateError("Ошибка изменения пароля. error {F268D35F-9697-4667-A4BA-6E57220A90EC}");

        IdentityResult result = await userManager.DeleteAsync(user.ApplicationUser);
        if (!result.Succeeded)
            return ResponseBaseModel.CreateError("Произошла непредвиденная ошибка при удалении пользователя.");

        return ResponseBaseModel.CreateSuccess("Данные пользователя удалены!");
    }

    /// <inheritdoc/>
    public async Task<UserBooleanResponseModel> UserHasPasswordAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new UserBooleanResponseModel() { Messages = user.Messages };

        TResponseModel<UserInfoModel[]> rest = await IdentityRepo.GetUsersIdentity([user.ApplicationUser.Id]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        return new()
        {
            UserInfo = rest.Response[0],
            Response = await userManager.HasPasswordAsync(user.ApplicationUser)
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> GetTwoFactorEnabledAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        return new() { Response = await userManager.GetTwoFactorEnabledAsync(user.ApplicationUser) };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetTwoFactorEnabledAsync(bool enabled_set, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        string msg;
        IdentityResult set2faResult = await userManager.SetTwoFactorEnabledAsync(user.ApplicationUser, enabled_set);
        if (!set2faResult.Succeeded)
        {
            return ResponseBaseModel.CreateError("Произошла непредвиденная ошибка при отключении 2FA.");
        }
        msg = $"Двухфакторная аутентификация для #{userId}/{user.ApplicationUser.Email} установлена в: {enabled_set}";
        LoggerRepo.LogInformation(msg);
        return ResponseBaseModel.CreateSuccess(msg);
    }

    /// <inheritdoc/>
    public async Task<UserBooleanResponseModel> IsEmailConfirmedAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        TResponseModel<UserInfoModel[]> rest = await IdentityRepo.GetUsersIdentity([user.ApplicationUser.Id]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        return new()
        {
            Response = rest.Response[0].EmailConfirmed,
            UserInfo = rest.Response[0]
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> GenerateChangeEmailTokenAsync(string newEmail, string baseAddress, string? userId = null)
    {
        if (!MailAddress.TryCreate(newEmail, out _))
            return ResponseBaseModel.CreateError($"Адрес e-mail `{newEmail}` имеет не корректный формат");

        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        //userId = await userManager.GetUserIdAsync(user.ApplicationUser);
        string code = await userManager.GenerateChangeEmailTokenAsync(user.ApplicationUser, newEmail);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        string callbackUrl = $"{baseAddress}?userId={userId}&email={newEmail}&code={code}";
        await emailSender.SendConfirmationLinkAsync(user.ApplicationUser, newEmail, HtmlEncoder.Default.Encode(callbackUrl));

        return ResponseBaseModel.CreateSuccess("Письмо с ссылкой для подтверждения изменения отправлено на ваш E-mail. Пожалуйста, проверьте свою электронную почту.");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ResetAuthenticatorKeyAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        await userManager.ResetAuthenticatorKeyAsync(user.ApplicationUser);
        string msg = $"Пользователь с идентификатором '{userId}' сбросил ключ приложения для аутентификации.";
        LoggerRepo.LogInformation(msg);
        await signInManager.RefreshSignInAsync(user.ApplicationUser);
        return ResponseBaseModel.CreateSuccess(msg);
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetUserNameAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        return new() { Response = user.ApplicationUser.UserName };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetPhoneNumberAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        return new() { Response = user.ApplicationUser.PhoneNumber };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> RefreshSignInAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        await signInManager.RefreshSignInAsync(user.ApplicationUser);

        return ResponseBaseModel.CreateSuccess("Вход выполнен");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<IEnumerable<UserLoginInfoModel>?>> GetUserLogins(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        IList<UserLoginInfo> data_logins = await userManager.GetLoginsAsync(user.ApplicationUser);
        return new()
        {
            Response = data_logins.Select(x => new UserLoginInfoModel(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName))
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetPasswordHashAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        string? passwordHash = null;
        if (userStore is IUserPasswordStore<ApplicationUser> userPasswordStore && httpContextAccessor.HttpContext is not null)
            passwordHash = await userPasswordStore.GetPasswordHashAsync(user.ApplicationUser, httpContextAccessor.HttpContext.RequestAborted);

        return new() { Response = passwordHash };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddLoginAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        ExternalLoginInfo? info = await signInManager.GetExternalLoginInfoAsync(user.ApplicationUser.Id);
        if (info is null)
            return ResponseBaseModel.CreateError("ExternalLoginInfo is null. error {6EFD4D81-8E30-472D-8356-3CF287639792}");

        IdentityResult result = await userManager.AddLoginAsync(user.ApplicationUser, info);
        if (!result.Succeeded)
            return ResponseBaseModel.CreateError("Ошибка: внешний логин не был добавлен. Внешние логины могут быть связаны только с одной учетной записью.");

        return ResponseBaseModel.CreateSuccess("Login is added");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> RemoveLoginAsync(string loginProvider, string providerKey, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        IdentityResult result = await userManager.RemoveLoginAsync(user.ApplicationUser, loginProvider, providerKey);
        if (!result.Succeeded)
            return ResponseBaseModel.CreateError("Ошибка удаления. error {832D1C29-D362-4238-AA88-C3E4E41A97FD}");
        await signInManager.RefreshSignInAsync(user.ApplicationUser);

        return ResponseBaseModel.CreateSuccess("Успешно удалено");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> VerifyTwoFactorTokenAsync(string verificationCode, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        bool is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(
           user.ApplicationUser, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2faTokenValid)
            return new(ResponseBaseModel.ErrorMessage("Ошибка: код подтверждения недействителен."));

        return new(ResponseBaseModel.SuccessMessage("Токен действителен"));
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> CountRecoveryCodesAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        return new() { Response = await userManager.CountRecoveryCodesAsync(user.ApplicationUser) };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<IEnumerable<string>?>> GenerateNewTwoFactorRecoveryCodesAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        return new() { Response = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user.ApplicationUser, 10) };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetAuthenticatorKeyAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        string? unformattedKey = await userManager.GetAuthenticatorKeyAsync(user.ApplicationUser);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user.ApplicationUser);
            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user.ApplicationUser);
        }

        return new()
        {
            Response = unformattedKey
        };
    }


    #region done
    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GeneratePasswordResetTokenAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        return await IdentityRepo.GeneratePasswordResetTokenAsync(user.ApplicationUser.Id);
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TryAddRolesToUser(IEnumerable<string> addRoles, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        return await IdentityRepo.TryAddRolesToUser(new() { RolesNames = [.. addRoles], UserId = user.ApplicationUser.Id });
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ChangePasswordAsync(string currentPassword, string newPassword, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        ResponseBaseModel res = await IdentityRepo.ChangePassword(new() { CurrentPassword = currentPassword, NewPassword = newPassword, UserId = user.ApplicationUser.Id });
        if (!res.Success())
            return res;

        user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        await signInManager.RefreshSignInAsync(user.ApplicationUser);
        return ResponseBaseModel.CreateSuccess($"Пользователю [`{user.ApplicationUser.Id}`/`{user.ApplicationUser.Email}`] успешно изменён пароль.");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddPasswordAsync(string password, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        ResponseBaseModel addPassRes = await IdentityRepo.AddPassword(new() { Password = password, UserId = user.ApplicationUser.Id });
        if (!addPassRes.Success())
            return addPassRes;
        user = await GetUser(userId);

        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        await signInManager.RefreshSignInAsync(user.ApplicationUser);
        return ResponseBaseModel.CreateSuccess("Пароль установлен");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ChangeEmailAsync(IdentityEmailTokenModel req)
    {
        ResponseBaseModel changeRes = await IdentityRepo.ChangeEmailAsync(req);
        if (!changeRes.Success())
            return changeRes;

        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь #{req.UserId} не найден");

        await signInManager.RefreshSignInAsync(user);
        return ResponseBaseModel.CreateSuccess("Благодарим вас за подтверждение изменения адреса электронной почты.");
    }


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
    #endregion
}