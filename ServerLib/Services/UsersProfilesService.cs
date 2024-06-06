////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using DbcLib;
using IdentityLib;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SharedLib;
using StackExchange.Redis;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;

namespace ServerLib;
#pragma warning disable CS9107
/// <summary>
/// Сервис работы с профилями пользователей
/// </summary>
public class UsersProfilesService(IEmailSender<ApplicationUser> emailSender, IDbContextFactory<IdentityAppDbContext> identityDbFactory, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserStore<ApplicationUser> userStore, IHttpContextAccessor httpContextAccessor, ILogger<UsersProfilesService> LoggerRepo) : GetUserServiceAbstract(httpContextAccessor, identityDbFactory), IUsersProfilesService
{
#pragma warning restore CS9107
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddPasswordAsync(string password, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
        {
            return new ResponseBaseModel()
            {
                Messages = user.Messages
            };
        }

        IdentityResult addPasswordResult = await userManager.AddPasswordAsync(user.ApplicationUser, password);

        if (!addPasswordResult.Succeeded)
        {
            return new ResponseBaseModel()
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
    public async Task<ResponseBaseModel> ChangeEmailAsync(string user_id, string newEmail, string token)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(user_id); ;
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь #{user_id} не найден");

        string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        IdentityResult result = await userManager.ChangeEmailAsync(user, newEmail, code);
        if (!result.Succeeded)
            return ResponseBaseModel.CreateError("Ошибка при смене электронной почты.");

        IdentityResult setUserNameResult = await userManager.SetUserNameAsync(user, newEmail);

        if (!setUserNameResult.Succeeded)
            return ResponseBaseModel.CreateError("Ошибка изменения имени пользователя.");

        await signInManager.RefreshSignInAsync(user);
        return ResponseBaseModel.CreateSuccess("Благодарим вас за подтверждение изменения адреса электронной почты.");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ChangePasswordAsync(string currentPassword, string newPassword, string? userId = null)
    {
        string msg;
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
        {
            return new ResponseBaseModel()
            {
                Messages = user.Messages
            };
        }

        IdentityResult changePasswordResult = await userManager.ChangePasswordAsync(user.ApplicationUser, currentPassword, newPassword);
        if (!changePasswordResult.Succeeded)
        {
            return new()
            {
                Messages = changePasswordResult.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}: {x.Description}]" }).ToList(),
            };
        }

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
        {
            return new UserBooleanResponseModel()
            {
                Messages = user.Messages,
                UserInfo = user.ApplicationUser is null
                ? null
                : (UserInfoModel)user.ApplicationUser
            };
        }

        string msg;
        if (!await userManager.CheckPasswordAsync(user.ApplicationUser, password))
        {
            msg = "Ошибка: Неправильный пароль. error {91A2600D-5EBF-4F79-83BE-28F6FA55301C}";
            LoggerRepo.LogError(msg);
            return (UserBooleanResponseModel)ResponseBaseModel.CreateError(msg);
        }

        return (UserBooleanResponseModel)ResponseBaseModel.CreateSuccess("Пароль проверку прошёл!");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteUserDataAsync(string password, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
        {
            return new ResponseBaseModel()
            {
                Messages = user.Messages
            };
        }

        UserBooleanResponseModel user_has_pass = await UserHasPasswordAsync(user.ApplicationUser.Id);

        if (!user_has_pass.Success() || user_has_pass.Response != true || !await userManager.CheckPasswordAsync(user.ApplicationUser, password))
            return ResponseBaseModel.CreateError("Ошибка изменения пароля. error {F268D35F-9697-4667-A4BA-6E57220A90EC}");

        IdentityResult result = await userManager.DeleteAsync(user.ApplicationUser);
        if (!result.Succeeded)
            return ResponseBaseModel.CreateError("Произошла непредвиденная ошибка при удалении пользователя.");

        return ResponseBaseModel.CreateSuccess("Данные пользователя удалены!");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel?>> FindByIdAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new TResponseModel<UserInfoModel?>() { Messages = user.Messages };

        return new() { Response = (UserInfoModel)user.ApplicationUser };
    }

    /// <inheritdoc/>
    public async Task<UserBooleanResponseModel> UserHasPasswordAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
        {
            return new UserBooleanResponseModel()
            {
                Messages = user.Messages,
                UserInfo = user.ApplicationUser is null
                ? null
                : (UserInfoModel)user.ApplicationUser
            };
        }
        return new UserBooleanResponseModel()
        {
            UserInfo = (UserInfoModel)user.ApplicationUser,
            Response = await userManager.HasPasswordAsync(user.ApplicationUser)
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> GetTwoFactorEnabledAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new TResponseModel<bool?>() { Messages = user.Messages };

        return new()
        {
            Response = await userManager.GetTwoFactorEnabledAsync(user.ApplicationUser)
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetTwoFactorEnabledAsync(bool enabled_set, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
        {

            return new ResponseBaseModel()
            {
                Messages = user.Messages
            };
        }

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
        {

            return new UserBooleanResponseModel()
            {
                Messages = user.Messages
            };
        }

        return new()
        {
            Response = await userManager.IsEmailConfirmedAsync(user.ApplicationUser),
            UserInfo = (UserInfoModel)user.ApplicationUser
        };
    }

    /// <inheritdoc/>
    public async Task<UserInfoModel?> FindByEmailAsync(string email)
    {
        ApplicationUser? user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return null;

        return new() { UserId = user.Id, Email = user.Email };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ResetPasswordAsync(string userId, string token, string newPassword)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
        {

            return new ResponseBaseModel()
            {
                Messages = user.Messages
            };
        }
        IdentityResult result = await userManager.ResetPasswordAsync(user.ApplicationUser, token, newPassword);
        if (!result.Succeeded)
            return new ResponseBaseModel()
            {
                Messages = result.Errors.Select(x => new ResultMessage()
                {
                    TypeMessage = ResultTypesEnum.Error,
                    Text = $"[{x.Code}: {x.Description}]"
                }).ToList()
            };

        return ResponseBaseModel.CreateSuccess("Пароль успешно сброшен");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> GenerateEmailConfirmationTokenAsync(string userEmail, string baseAddress)
    {
        string msg = "Письмо с подтверждением отправлено. Пожалуйста, проверьте свою электронную почту.";
        ApplicationUser? user = await userManager.FindByEmailAsync(userEmail);
        if (user is null)
            return ResponseBaseModel.CreateError(msg);

        string userId = await userManager.GetUserIdAsync(user);
        string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        string callbackUrl = $"{baseAddress}?userId={userId}&code={code}";
        await emailSender.SendConfirmationLinkAsync(user, userEmail, HtmlEncoder.Default.Encode(callbackUrl));
        return ResponseBaseModel.CreateInfo(msg);
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> GenerateChangeEmailTokenAsync(string newEmail, string baseAddress, string? userId = null)
    {
        if (!MailAddress.TryCreate(newEmail, out _))
            return ResponseBaseModel.CreateError($"Адрес e-mail `{newEmail}` имеет не корректный формат");

        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new TResponseModel<string?>() { Messages = user.Messages };

        userId = await userManager.GetUserIdAsync(user.ApplicationUser);
        string code = await userManager.GenerateChangeEmailTokenAsync(user.ApplicationUser, newEmail);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        string callbackUrl = $"{baseAddress}?userId={userId}&email={newEmail}&code={code}";
        await emailSender.SendConfirmationLinkAsync(user.ApplicationUser, newEmail, HtmlEncoder.Default.Encode(callbackUrl));

        return ResponseBaseModel.CreateSuccess("Ссылка для подтверждения изменения отправлено на ваш E-mail. Пожалуйста, проверьте свою электронную почту.");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ResetAuthenticatorKeyAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new ResponseBaseModel() { Messages = user.Messages };

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
            return new ResponseBaseModel() { Messages = user.Messages };

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetUserNameAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new TResponseModel<string?>() { Messages = user.Messages };

        return new TResponseModel<string?>() { Response = await userManager.GetUserNameAsync(user.ApplicationUser) };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetPhoneNumberAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new TResponseModel<string?>() { Messages = user.Messages };

        return new TResponseModel<string?>() { Response = await userManager.GetPhoneNumberAsync(user.ApplicationUser) };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> RefreshSignInAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new ResponseBaseModel() { Messages = user.Messages };

        await signInManager.RefreshSignInAsync(user.ApplicationUser);

        return ResponseBaseModel.CreateSuccess("Вход выполнен");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<IEnumerable<UserLoginInfoModel>?>> GetUserLogins(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new TResponseModel<IEnumerable<UserLoginInfoModel>?>() { Messages = user.Messages };

        IList<UserLoginInfo> data_logins = await userManager.GetLoginsAsync(user.ApplicationUser);
        return new TResponseModel<IEnumerable<UserLoginInfoModel>?>()
        {
            Response = data_logins.Select(x => new UserLoginInfoModel(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName))
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetPasswordHashAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new TResponseModel<string?>() { Messages = user.Messages };

        string? passwordHash = null;
        if (userStore is IUserPasswordStore<ApplicationUser> userPasswordStore && httpContextAccessor.HttpContext is not null)
            passwordHash = await userPasswordStore.GetPasswordHashAsync(user.ApplicationUser, httpContextAccessor.HttpContext.RequestAborted);

        return new TResponseModel<string?>() { Response = passwordHash };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddLoginAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new ResponseBaseModel() { Messages = user.Messages };

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
            return new ResponseBaseModel() { Messages = user.Messages };

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
            return new TResponseModel<bool?>() { Messages = user.Messages };

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
            return new TResponseModel<int?>() { Messages = user.Messages };

        return new TResponseModel<int?>() { Response = await userManager.CountRecoveryCodesAsync(user.ApplicationUser) };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<IEnumerable<string>?>> GenerateNewTwoFactorRecoveryCodesAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new TResponseModel<IEnumerable<string>?>() { Messages = user.Messages };

        return new() { Response = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user.ApplicationUser, 10) };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetAuthenticatorKeyAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new TResponseModel<string?>() { Messages = user.Messages };

        string? unformattedKey = await userManager.GetAuthenticatorKeyAsync(user.ApplicationUser);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user.ApplicationUser);
            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user.ApplicationUser);
        }

        return new TResponseModel<string?>()
        {
            Response = unformattedKey
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GeneratePasswordResetTokenAsync(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new TResponseModel<string?>() { Messages = user.Messages };

        return new TResponseModel<string?>()
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
            return new ResponseBaseModel() { Messages = user.Messages };

        string code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(pass_reset_token));
        string callbackUrl = $"{baseAddress}?code={code}";
        await emailSender.SendPasswordResetLinkAsync(user.ApplicationUser, email, HtmlEncoder.Default.Encode(callbackUrl));

        return ResponseBaseModel.CreateSuccess("Письмо с токеном отправлено на Email");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TryAddRolesToUser(IEnumerable<string> addRoles, string? userId = null)
    {
        addRoles = addRoles.Where(x => !string.IsNullOrWhiteSpace(x));
        if (!addRoles.Any())
            return ResponseBaseModel.CreateError("Не указаны роли для добавления");

        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new ResponseBaseModel() { Messages = user.Messages };

        // роли, которые есть в БД (поиск по именам)
        string?[] roles = await roleManager.Roles
            .Where(x => addRoles.Any(y => y == x.Name))
            .Select(x => x.Name)
            .ToArrayAsync();

        // роли, которых нет в бд вообще
        IList<string> roles_names = addRoles
            .Where(x => !roles.Any(y => y?.Equals(x) == true))
            .ToList();

        foreach (string r in roles_names)
            try
            {
                await roleManager.CreateAsync(new ApplicationRole() { Name = r, NormalizedName = userManager.NormalizeName(r) });
            }
            catch (Exception ex)
            {
                LoggerRepo.LogError(ex, $"Ошибка создания роли '{r}'. error D0BD6816-A325-421C-BF75-EA402D3704C3");
            }

        IList<string> user_roles = await userManager.GetRolesAsync(user.ApplicationUser);

        // роли, которые требуется добавить пользователю
        roles_names = addRoles.Where(x => !user_roles.Any(y => y.Equals(x) == true)).ToList();
        if (roles_names.Count > 0)
            await userManager.AddToRolesAsync(user.ApplicationUser, roles_names);

        return ResponseBaseModel.CreateSuccess($"Добавлено {roles_names.Count} ролей пользователю");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddRoleToUser(string role_name, string user_email)
    {
        ApplicationRole? role_db = await roleManager.FindByNameAsync(role_name);
        if (role_db is null)
            return ResponseBaseModel.CreateError($"Роль с именем '{role_name}' не найдена в БД");

        ApplicationUser? user_db = await userManager.FindByEmailAsync(user_email);
        if (user_db is null)
            return ResponseBaseModel.CreateError($"Пользователь `{user_email}` не найден в БД");

        if (await userManager.IsInRoleAsync(user_db, role_name))
            return ResponseBaseModel.CreateWarning($"Роль '{role_name}' у пользователя '{user_email}' уже присутствует.");

        IdentityResult ir = await userManager.AddToRoleAsync(user_db, role_name);

        if (ir.Succeeded)
            return ResponseBaseModel.CreateSuccess($"Пользователю '{user_email}' добавлена роль '{role_name}'");

        return new ResponseBaseModel()
        {
            Messages = ir.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}: {x.Description}]" }).ToList()
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteRoleFromUser(string role_name, string user_email)
    {
        ApplicationRole? role_db = await roleManager.FindByNameAsync(role_name);
        if (role_db is null)
            return ResponseBaseModel.CreateError($"Роль с именем '{role_name}' не найдена в БД");

        ApplicationUser? user_db = await userManager.FindByEmailAsync(user_email);
        if (user_db is null)
            return ResponseBaseModel.CreateError($"Пользователь `{user_email}` не найден в БД");

        if (!await userManager.IsInRoleAsync(user_db, role_name))
            return ResponseBaseModel.CreateWarning($"Роль '{role_name}' у пользователя '{user_email}' отсутствует.");

        IdentityResult ir = await userManager.RemoveFromRoleAsync(user_db, role_name);

        if (ir.Succeeded)
            return ResponseBaseModel.CreateSuccess($"Пользователь '{user_email}' исключён из роли '{role_name}'");

        return new ResponseBaseModel()
        {
            Messages = ir.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}: {x.Description}]" }).ToList()
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteRole(string role_name)
    {
        ApplicationRole? role_db = await roleManager.FindByNameAsync(role_name);
        if (role_db is null)
            return ResponseBaseModel.CreateError($"Роль #{role_name} не найдена в БД");

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        var users_linked =
           await (from link in identityContext.UserRoles.Where(x => x.RoleId == role_db.Id)
                  join user in identityContext.Users on link.UserId equals user.Id
                  select new { user.Id, user.Email }).ToArrayAsync();

        if (users_linked.Length != 0)
            return ResponseBaseModel.CreateError($"Роль '{role_db.Name}' нельзя удалить! Предварительно исключите из неё пользователей: {string.Join("; ", users_linked.Select(x => $"[{x.Email}]"))};");

        IdentityResult ir = await roleManager.DeleteAsync(role_db);

        if (ir.Succeeded)
            ResponseBaseModel.CreateSuccess($"Роль '{role_db.Name}' успешно удалена!");

        return new ResponseBaseModel()
        {
            Messages = ir.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}: {x.Description}]" }).ToList()
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CateNewRole(string role_name)
    {
        role_name = role_name.Trim();
        if (string.IsNullOrEmpty(role_name))
            return ResponseBaseModel.CreateError("Не указано имя роли");
        ApplicationRole? role_db = await roleManager.FindByNameAsync(role_name);
        if (role_db is not null)
            return ResponseBaseModel.CreateWarning($"Роль '{role_db.Name}' уже существует");

        role_db = new ApplicationRole(role_name);
        IdentityResult ir = await roleManager.CreateAsync(role_db);

        if (ir.Succeeded)
            return ResponseBaseModel.CreateSuccess($"Роль '{role_name}' успешно создана");

        return new ResponseBaseModel()
        {
            Messages = ir.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}: {x.Description}]" }).ToList()
        };
    }

    /// <inheritdoc/>
    public async Task<RolesPaginationModel> FindRolesAsync(FindWithOwnedRequestModel req)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        IQueryable<ApplicationRole> q = identityContext.Roles
           .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.OwnerId))
            q = q.Where(x => identityContext.UserRoles.Any(y => x.Id == y.RoleId && req.OwnerId == y.UserId));

        if (!string.IsNullOrWhiteSpace(req.FindQuery))
            q = q.Where(x => EF.Functions.Like(x.NormalizedName, $"%{roleManager.NormalizeKey(req.FindQuery)}%") || x.Id == req.FindQuery);

        int total = q.Count();
        q = q.Skip(req.PageNum * req.PageSize).Take(req.PageSize);
        var roles = await
            q.Select(x => new
            {
                x.Id,
                x.Name,
                x.Title,
                UsersCount = identityContext.UserRoles.Count(z => z.RoleId == x.Id)
            })
            .ToArrayAsync();

        return new RolesPaginationModel()
        {
            Roles = roles.Select(x => new RoleInfoModel() { Id = x.Id, Name = x.Name, Title = x.Title, UsersCount = x.UsersCount }).ToList(),
            TotalRowsCount = total,
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<RoleInfoModel?>> GetRole(string role_id)
    {
        ApplicationRole? role_db = await roleManager.FindByIdAsync(role_id);
        if (role_db is null)
            return new() { Messages = ResponseBaseModel.ErrorMessage($"Роль #{role_id} не найдена в БД") };
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        return new()
        {
            Response = new RoleInfoModel()
            {
                Id = role_id,
                Name = role_db.Name,
                Title = role_db.Title,
                UsersCount = await identityContext.UserRoles.CountAsync(x => x.RoleId == role_id)
            }
        };
    }

    /// <inheritdoc/>
    public async Task<UsersInfoPaginationModel> FindUsersAsync(FindWithOwnedRequestModel req)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        IQueryable<ApplicationUser> q = identityContext.Users
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.OwnerId))
            q = q.Where(x => identityContext.UserRoles.Any(y => x.Id == y.UserId && req.OwnerId == y.RoleId));

        if (!string.IsNullOrWhiteSpace(req.FindQuery))
            q = q.Where(x => EF.Functions.Like(x.Email, $"%{req.FindQuery}%") || EF.Functions.Like(x.UserName, $"%{req.FindQuery}%") || x.Id == req.FindQuery);

        int total = q.Count();
        q = q.Skip(req.PageNum * req.PageSize).Take(req.PageSize);
        var users = await q
            .Select(x => new
            {
                x.Id,
                x.UserName,
                x.Email,
                x.PhoneNumber,
                x.TelegramId,
                x.EmailConfirmed,
                x.LockoutEnd,
                x.LockoutEnabled,
                x.AccessFailedCount
            })
            .ToArrayAsync();
        string[] users_ids = users.Select(x => x.Id).ToArray();
        var roles =
           await (from link in identityContext.UserRoles.Where(x => users_ids.Contains(x.UserId))
                  join role in identityContext.Roles on link.RoleId equals role.Id
                  select new { RoleName = role.Name, link.UserId }).ToArrayAsync();

        return new()
        {
            UsersInfo = users.Select(x => UserInfoModel.Build(x.Id, x.Email, x.UserName, x.PhoneNumber, x.TelegramId, x.EmailConfirmed, x.LockoutEnd, x.LockoutEnabled, x.AccessFailedCount, roles.Where(y => y.UserId == x.Id).Select(z => z.RoleName).ToArray())).ToList(),
            TotalRowsCount = total,
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetLockUser(string userId, bool locketSet)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new ResponseBaseModel() { Messages = user.Messages };

        await userManager.SetLockoutEndDateAsync(user.ApplicationUser, locketSet ? DateTimeOffset.MaxValue : null);
        return ResponseBaseModel.CreateSuccess($"Пользователь успешно [{user.ApplicationUser.Email}] {(locketSet ? "заблокирован" : "разблокирован")}");
    }

    /// <inheritdoc/>
    public async Task<ClaimBaseModel[]> GetClaims(ClaimAreasEnum claimArea, string ownerId)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        return claimArea switch
        {
            ClaimAreasEnum.ForRole => await identityContext.RoleClaims.Where(x => x.RoleId == ownerId).Select(x => new ClaimBaseModel() { Id = x.Id, ClaimType = x.ClaimType, ClaimValue = x.ClaimValue }).ToArrayAsync(),
            ClaimAreasEnum.ForUser => await identityContext.UserClaims.Where(x => x.UserId == ownerId).Select(x => new ClaimBaseModel() { Id = x.Id, ClaimType = x.ClaimType, ClaimValue = x.ClaimValue }).ToArrayAsync(),
            _ => throw new NotImplementedException("error {61909910-B126-4204-8AE6-673E11D49BCD}")
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ClaimUpdateOrCreate(ClaimModel claim, ClaimAreasEnum claimArea)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();

        switch (claimArea)
        {
            case ClaimAreasEnum.ForRole:
                IdentityRoleClaim<string>? claim_role_db;
                if (claim.Id < 1)
                {
                    claim_role_db = new IdentityRoleClaim<string>() { RoleId = claim.OwnerId, ClaimType = claim.ClaimType, ClaimValue = claim.ClaimValue };
                    await identityContext.RoleClaims.AddAsync(claim_role_db);
                }
                else
                {
                    claim_role_db = await identityContext.RoleClaims.FirstOrDefaultAsync(x => x.RoleId == claim.OwnerId);
                    if (claim_role_db is null)
                        return ResponseBaseModel.CreateError($"Claim #{claim.OwnerId} не найден в БД");
                    else if (claim_role_db.ClaimType?.Equals(claim.ClaimType) == true && claim_role_db.ClaimValue?.Equals(claim.ClaimValue) == true)
                        return ResponseBaseModel.CreateInfo($"Claim #{claim.OwnerId} не изменён");

                    claim_role_db.ClaimType = claim.ClaimType;
                    claim_role_db.ClaimValue = claim.ClaimValue;
                    identityContext.RoleClaims.Update(claim_role_db);
                }

                break;
            case ClaimAreasEnum.ForUser:
                IdentityUserClaim<string>? claim_user_db;

                if (claim.Id < 1)
                {
                    claim_user_db = new IdentityUserClaim<string>() { UserId = claim.OwnerId, ClaimType = claim.ClaimType, ClaimValue = claim.ClaimValue };
                    await identityContext.UserClaims.AddAsync(claim_user_db);
                }
                else
                {
                    claim_user_db = await identityContext.UserClaims.FirstOrDefaultAsync(x => x.UserId == claim.OwnerId);
                    if (claim_user_db is null)
                        return ResponseBaseModel.CreateError($"Claim #{claim.OwnerId} не найден в БД");
                    else if (claim_user_db.ClaimType?.Equals(claim.ClaimType) == true && claim_user_db.ClaimValue?.Equals(claim.ClaimValue) == true)
                        return ResponseBaseModel.CreateInfo($"Claim #{claim.OwnerId} не изменён");

                    claim_user_db.ClaimType = claim.ClaimType;
                    claim_user_db.ClaimValue = claim.ClaimValue;
                    identityContext.UserClaims.Update(claim_user_db);
                }

                break;
            default:
                throw new NotImplementedException("error {33A20922-0E76-421F-B2C4-109B7A420827}");
        }

        await identityContext.SaveChangesAsync();

        return ResponseBaseModel.CreateSuccess("Запрос успешно обработан");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ClaimDelete(ClaimAreasEnum claimArea, int id)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();

        switch (claimArea)
        {
            case ClaimAreasEnum.ForRole:
                IdentityRoleClaim<string>? claim_role_db = await identityContext.RoleClaims.FirstOrDefaultAsync(x => x.Id == id);

                if (claim_role_db is null)
                    return ResponseBaseModel.CreateWarning($"Claim #{id} не найден в БД");

                identityContext.RoleClaims.Remove(claim_role_db);
                break;
            case ClaimAreasEnum.ForUser:
                IdentityUserClaim<string>? claim_user_db = await identityContext.UserClaims.FirstOrDefaultAsync(x => x.Id == id);

                if (claim_user_db is null)
                    return ResponseBaseModel.CreateError($"Claim #{id} не найден в БД");

                identityContext.UserClaims.Remove(claim_user_db);
                break;
            default:
                throw new NotImplementedException("error {7F5317DC-EA89-47C3-BE2A-8A90838A113C}");
        }

        await identityContext.SaveChangesAsync();

        return ResponseBaseModel.CreateSuccess("Claim успешно удалён");
    }
}
internal record IdentityUserRecord(string? Email, bool EmailConfirmed, string? PhoneNumber, bool PhoneNumberConfirmed, bool TwoFactorEnabled, DateTimeOffset? LockoutEnd, bool LockoutEnabled, int AccessFailedCount);