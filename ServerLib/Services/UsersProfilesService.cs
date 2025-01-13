////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
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
    IDbContextFactory<IdentityAppDbContext> identityDbFactory,
    UserManager<ApplicationUser> userManager,
    IIdentityTransmission IdentityRepo,
    SignInManager<ApplicationUser> signInManager,
    IUserStore<ApplicationUser> userStore,
    IHttpContextAccessor httpContextAccessor,
    ILogger<UsersProfilesService> LoggerRepo) : GetUserServiceAbstract(httpContextAccessor, userManager, LoggerRepo), IUsersProfilesService
{
#pragma warning restore CS9107
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddPasswordAsync(string password, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        IdentityResult addPasswordResult = await userManager.AddPasswordAsync(user.ApplicationUser, password);

        if (!addPasswordResult.Succeeded)
        {
            return new()
            {
                Messages = addPasswordResult.Errors.Select(e => new ResultMessage()
                {
                    TypeMessage = ResultTypesEnum.Error,
                    Text = $"[{e.Code}: {e.Description}]"
                }).ToList()
            };
        }

        await signInManager.RefreshSignInAsync(user.ApplicationUser);
        return ResponseBaseModel.CreateSuccess("Пароль установлен");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ChangePasswordAsync(string currentPassword, string newPassword, string? userId = null)
    {
        string msg;
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        IdentityResult changePasswordResult = await userManager.ChangePasswordAsync(user.ApplicationUser, currentPassword, newPassword);
        if (!changePasswordResult.Succeeded)
            return new()
            {
                Messages = [.. changePasswordResult.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}: {x.Description}]" })],
            };

        await signInManager.RefreshSignInAsync(user.ApplicationUser);
        msg = $"Пользователю [`{user.ApplicationUser.Id}`/`{user.ApplicationUser.Email}`] успешно изменён пароль.";
        LoggerRepo.LogInformation(msg);
        return ResponseBaseModel.CreateSuccess(msg);
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
    public async Task<ResponseBaseModel> SetPhoneNumberAsync(string? phoneNumber, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };
        // TODO: XXX
        throw new NotImplementedException();
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

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GeneratePasswordResetTokenAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        return new()
        {
            Response = await userManager.GeneratePasswordResetTokenAsync(user.ApplicationUser)
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SendPasswordResetLinkAsync(string email, string baseAddress, string pass_reset_token, string? userId = null)
    {
        if (!MailAddress.TryCreate(email, out _))
            return ResponseBaseModel.CreateError($"email `{email}` имеет не корректный формат. error {{4EE55201-8367-433D-9766-ABDE15B7BC04}}");

        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        string code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(pass_reset_token));
        string callbackUrl = $"{baseAddress}?code={code}";
        await emailSender.SendPasswordResetLinkAsync(user.ApplicationUser, email, HtmlEncoder.Default.Encode(callbackUrl));

        return ResponseBaseModel.CreateSuccess("Письмо с токеном отправлено на Email");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TryAddRolesToUser(IEnumerable<string> addRoles, string? userId = null)
    {
        addRoles = addRoles
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .DistinctBy(x => x.ToLower());

        if (!addRoles.Any())
            return ResponseBaseModel.CreateError("Не указаны роли для добавления");

        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new() { Messages = user.Messages };

        string[] roles_for_add_normalized = addRoles.Select(r => userManager.NormalizeName(r)).ToArray();

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        // роли, которые есть в БД
        string?[] roles_that_are_in_db = await identityContext.Roles
            .Where(x => roles_for_add_normalized.Contains(x.NormalizedName))
            .Select(x => x.Name)
            .ToArrayAsync();

        // роли, которых не хватает в бд
        string[] roles_that_need_add_in_db = addRoles
            .Where(x => !roles_that_are_in_db.Any(y => y?.Equals(x, StringComparison.OrdinalIgnoreCase) == true))
            .ToArray();

        if (roles_that_need_add_in_db.Length != 0)
        {
            await identityContext
                .AddRangeAsync(roles_that_need_add_in_db.Select(r => new ApplicationRole() { Name = r, Title = r, NormalizedName = userManager.NormalizeName(r) }));
            await identityContext.SaveChangesAsync();
        }

        IList<string> user_roles = await userManager.GetRolesAsync(user.ApplicationUser);

        // роли, которые требуется добавить пользователю
        roles_that_need_add_in_db = roles_for_add_normalized
            .Where(x => !user_roles.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase) == true))
            .ToArray();

        if (roles_that_need_add_in_db.Length != 0)
        { // добавляем пользователю ролей
            roles_that_need_add_in_db = await identityContext
                .Roles
                .Where(x => roles_that_need_add_in_db.Contains(x.NormalizedName))
                .Select(x => x.Id)
                .ToArrayAsync();

            await identityContext.AddRangeAsync(roles_that_need_add_in_db.Select(x => new IdentityUserRole<string>() { RoleId = x, UserId = userId! }));
            await identityContext.SaveChangesAsync();
        }
        return ResponseBaseModel.CreateSuccess($"Добавлено {addRoles.Count()} ролей пользователю");
    }



    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ChangeEmailAsync(IdentityEmailTokenModel req)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь #{req.UserId} не найден");

        ResponseBaseModel changeRes = await IdentityRepo.ChangeEmailAsync(req);
        if (!changeRes.Success())
            return changeRes;

        await signInManager.RefreshSignInAsync(user);
        return ResponseBaseModel.CreateSuccess("Благодарим вас за подтверждение изменения адреса электронной почты.");
    }
}