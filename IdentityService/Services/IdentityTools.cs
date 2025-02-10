////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Security.Claims;
using System.Net.Mail;
using IdentityLib;
using System.Text;
using SharedLib;
using Newtonsoft.Json;

namespace IdentityService;

/// <summary>
/// IdentityTools
/// </summary>
public class IdentityTools(
    IServiceScopeFactory serviceScopeFactory,
    IEmailSender<ApplicationUser> emailSender,
    //IUserStore<ApplicationUser> userStore,
    //RoleManager<ApplicationRole> roleManager,
    //UserManager<ApplicationUser> userManager,
    IManualCustomCacheService memCache,
    IMailProviderService mailRepo,
    ILogger<IdentityTools> loggerRepo,
    ITelegramTransmission tgRemoteRepo,
    IDbContextFactory<IdentityAppDbContext> identityDbFactory) : IIdentityTools
{
    /// <inheritdoc/>
    public async Task<TResponseModel<string>> CheckToken2FA(CheckToken2FARequestModel req)
    {
        MemCachePrefixModel pref = new(GlobalStaticConstants.Routes.TWOFACTOR_CONTROLLER_NAME, GlobalStaticConstants.Routes.ALIAS_CONTROLLER_NAME);
        string? userId = await memCache.GetStringValueAsync(pref, req.UserAlias);
        if (string.IsNullOrWhiteSpace(userId))
            return new() { Messages = [new() { Text = "Алиас пользователя отсутствует!", TypeMessage = ResultTypesEnum.Error }] };

        await memCache.RemoveAsync(pref, req.UserAlias);

        string? token = await memCache.GetStringValueAsync(new MemCachePrefixModel(GlobalStaticConstants.Routes.TWOFACTOR_CONTROLLER_NAME, GlobalStaticConstants.Routes.TOKEN_CONTROLLER_NAME), userId);
        if (string.IsNullOrWhiteSpace(token))
            return new() { Messages = [new() { Text = "Токен 2FA отсутствует!", TypeMessage = ResultTypesEnum.Error }] };

        if (!req.Token.Equals(token))
            return new() { Messages = [new() { Text = "Токен не верный!", TypeMessage = ResultTypesEnum.Error }] };

        await memCache.RemoveAsync(new MemCachePrefixModel(GlobalStaticConstants.Routes.TWOFACTOR_CONTROLLER_NAME, GlobalStaticConstants.Routes.TOKEN_CONTROLLER_NAME), userId);
        return new() { Response = userId, Messages = [new() { Text = "Токен верный", TypeMessage = ResultTypesEnum.Success }] };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string>> ReadToken2FA(string userId)
    {
        return new() { Response = await memCache.GetStringValueAsync(new MemCachePrefixModel(GlobalStaticConstants.Routes.TWOFACTOR_CONTROLLER_NAME, GlobalStaticConstants.Routes.TOKEN_CONTROLLER_NAME), userId) };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string>> GenerateToken2FA(string userId)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(userId); ;
        if (user is null)
            return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = $"Пользователь #{userId} не найден" }] };

        IList<string> providers = await userManager.GetValidTwoFactorProvidersAsync(user);
        if (!providers.Contains("Email"))
            return (TResponseModel<string>)ResponseBaseModel.CreateError("Invalid 2-Step Verification Provider.");

        string token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

        if (!string.IsNullOrWhiteSpace(user.Email))
            await mailRepo.SendEmailAsync(user.Email, "Authentication 2FA token", token);

        await memCache.SetStringAsync(new MemCachePrefixModel(GlobalStaticConstants.Routes.TWOFACTOR_CONTROLLER_NAME, GlobalStaticConstants.Routes.TOKEN_CONTROLLER_NAME), userId, token, TimeSpan.FromMinutes(5));

        string aliasToken = Guid.NewGuid().ToString().Replace("-", "").Replace("{", "").Replace("}", "");
        await memCache.SetStringAsync(new MemCachePrefixModel(GlobalStaticConstants.Routes.TWOFACTOR_CONTROLLER_NAME, GlobalStaticConstants.Routes.ALIAS_CONTROLLER_NAME), aliasToken, userId, TimeSpan.FromMinutes(5));

        return new() { Response = aliasToken };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<IEnumerable<UserLoginInfoModel>>> GetUserLogins(string userId)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(userId); ;
        if (user is null)
            return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = $"Пользователь #{userId} не найден" }] };

        IList<UserLoginInfo> data_logins = await userManager.GetLoginsAsync(user);
        return new()
        {
            Response = data_logins.Select(x => new UserLoginInfoModel(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName))
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CheckUserPassword(IdentityPasswordModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = $"Пользователь #{req.UserId} не найден" }] };

        string msg;
        if (!await userManager.CheckPasswordAsync(user, req.Password))
        {
            msg = "Ошибка: Неправильный пароль. error {91A2600D-5EBF-4F79-83BE-28F6FA55301C}";
            loggerRepo.LogError(msg);
            return ResponseBaseModel.CreateError(msg);
        }

        return ResponseBaseModel.CreateSuccess("Пароль проверку прошёл!");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteUserData(DeleteUserDataRequestModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = $"Пользователь #{req.UserId} не найден" }] };

        bool user_has_pass = await userManager.HasPasswordAsync(user);

        if (!user_has_pass || !await userManager.CheckPasswordAsync(user, req.Password))
            return ResponseBaseModel.CreateError("Ошибка изменения пароля. error {F268D35F-9697-4667-A4BA-6E57220A90EC}");

        IdentityResult result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return ResponseBaseModel.Create(result.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}:{x.Description}]" }));

        return ResponseBaseModel.CreateSuccess("Данные пользователя удалены!");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> UserHasPassword(string userId)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(userId); ;
        if (user is null)
            return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = $"Пользователь #{userId} не найден" }] };

        return new()
        {
            Response = await userManager.HasPasswordAsync(user)
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> GetTwoFactorEnabled(string userId)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(userId); ;
        if (user is null)
            return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = $"Пользователь #{userId} не найден" }] };

        return new() { Response = await userManager.GetTwoFactorEnabledAsync(user) };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetTwoFactorEnabled(SetTwoFactorEnabledRequestModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь #{req.UserId} не найден");

        string msg;
        IdentityResult set2faResult = await userManager.SetTwoFactorEnabledAsync(user, req.EnabledSet);
        if (!set2faResult.Succeeded)
        {
            return ResponseBaseModel.CreateError("Произошла непредвиденная ошибка при отключении 2FA.");
        }
        msg = $"Двухфакторная аутентификация для #{req.UserId}/{user.Email} установлена в: {req.EnabledSet}";
        loggerRepo.LogInformation(msg);
        return ResponseBaseModel.CreateSuccess(msg);
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ResetAuthenticatorKey(string userId)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(userId); ;
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь #{userId} не найден");

        IdentityResult res = await userManager.ResetAuthenticatorKeyAsync(user);

        if (!res.Succeeded)
            return ResponseBaseModel.Create(res.Errors.Select(x => new ResultMessage() { Text = $"[{x.Code}:{x.Description}]", TypeMessage = ResultTypesEnum.Error }));

        string msg = $"Пользователь с идентификатором '{userId}' сбросил ключ приложения для аутентификации.";
        loggerRepo.LogInformation(msg);

        return ResponseBaseModel.CreateSuccess(msg);
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> RemoveLoginForUser(RemoveLoginRequestModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь #{req.UserId} не найден");

        IdentityResult result = await userManager.RemoveLoginAsync(user, req.LoginProvider, req.ProviderKey);
        if (!result.Succeeded)
            return new() { Messages = result.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}:{x.Description}]" }).ToList() };

        return ResponseBaseModel.CreateSuccess("Успешно удалено");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> VerifyTwoFactorToken(VerifyTwoFactorTokenRequestModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return new() { Messages = [new() { Text = $"Пользователь #{req.UserId} не найден", TypeMessage = ResultTypesEnum.Error }] };

        bool is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(
           user, userManager.Options.Tokens.AuthenticatorTokenProvider, req.VerificationCode);

        return is2faTokenValid
            ? ResponseBaseModel.CreateSuccess("Токен действителен")
            : ResponseBaseModel.CreateError("Ошибка: код подтверждения недействителен");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> CountRecoveryCodes(string userId)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(userId); ;
        if (user is null)
            return new() { Messages = [new() { Text = $"Пользователь #{userId} не найден", TypeMessage = ResultTypesEnum.Error }] };

        return new() { Response = await userManager.CountRecoveryCodesAsync(user) };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> GenerateChangeEmailToken(GenerateChangeEmailTokenRequestModel req)
    {

        if (!MailAddress.TryCreate(req.NewEmail, out _))
            return ResponseBaseModel.CreateError($"Адрес e-mail `{req.NewEmail}` имеет не корректный формат");

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return new() { Messages = [new() { Text = $"Пользователь #{req.UserId} не найден", TypeMessage = ResultTypesEnum.Error }] };

        string code = await userManager.GenerateChangeEmailTokenAsync(user, req.NewEmail);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        string callbackUrl = $"{req.BaseAddress}?userId={user.Id}&email={req.NewEmail}&code={code}";
        await emailSender.SendConfirmationLinkAsync(user, req.NewEmail, HtmlEncoder.Default.Encode(callbackUrl));

        return ResponseBaseModel.CreateSuccess("Письмо с ссылкой для подтверждения изменения адреса почты отправлено на ваш E-mail. Пожалуйста, проверьте свою электронную почту.");

    }

    /// <inheritdoc/>
    public async Task<TResponseModel<IEnumerable<string>?>> GenerateNewTwoFactorRecoveryCodes(GenerateNewTwoFactorRecoveryCodesRequestModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return new() { Messages = [new() { Text = $"Пользователь #{req.UserId} не найден", TypeMessage = ResultTypesEnum.Error }] };

        return new() { Response = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, req.Number) };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetAuthenticatorKey(string userId)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(userId); ;
        if (user is null)
            return new() { Messages = [new() { Text = $"Пользователь #{userId} не найден", TypeMessage = ResultTypesEnum.Error }] };

        string? unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        }

        return new()
        {
            Response = unformattedKey
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GeneratePasswordResetToken(string userId)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(userId); ;
        if (user is null)
            return new() { Messages = [new() { Text = $"Пользователь #{userId} не найден", TypeMessage = ResultTypesEnum.Error }] };

        return new()
        {
            Response = await userManager.GeneratePasswordResetTokenAsync(user)
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SendPasswordResetLink(SendPasswordResetLinkRequestModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IEmailSender<ApplicationUser> emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender<ApplicationUser>>();

        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь #{req.UserId} не найден");

        if (!MailAddress.TryCreate(user.Email, out _))
            return ResponseBaseModel.CreateError($"email `{user.Email}` имеет не корректный формат. error {{4EE55201-8367-433D-9766-ABDE15B7BC04}}");

        string code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(req.ResetToken));
        string callbackUrl = $"{req.BaseAddress}?code={code}";
        await emailSender.SendPasswordResetLinkAsync(user, user.Email, HtmlEncoder.Default.Encode(callbackUrl));

        return ResponseBaseModel.CreateSuccess("Письмо с токеном отправлено на Email");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ChangePassword(IdentityChangePasswordModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь #{req.UserId} не найден");

        string msg;
        IdentityResult changePasswordResult = await userManager.ChangePasswordAsync(user, req.CurrentPassword, req.NewPassword);
        if (!changePasswordResult.Succeeded)
            return new()
            {
                Messages = [.. changePasswordResult.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}: {x.Description}]" })],
            };

        msg = $"Пользователю [`{user.Id}`/`{user.Email}`] успешно изменён пароль.";
        loggerRepo.LogInformation(msg);
        return ResponseBaseModel.CreateSuccess(msg);
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddPassword(IdentityPasswordModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь #{req.UserId} не найден");

        IdentityResult addPasswordResult = await userManager.AddPasswordAsync(user, req.Password);
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

        return ResponseBaseModel.CreateSuccess("Пароль установлен");
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<UserInfoModel>> SelectUsersOfIdentity(TPaginationRequestModel<SimpleBaseRequestModel> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();
        IQueryable<ApplicationUser> q = identityContext.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
        {
            req.Payload.SearchQuery = req.Payload.SearchQuery.ToUpper();
            q = q.Where(x => (x.NormalizedEmail != null && x.NormalizedEmail.Contains(req.Payload.SearchQuery)) ||
            (x.NormalizedUserName != null && x.NormalizedUserName.Contains(req.Payload.SearchQuery)) ||
            (x.NormalizedFirstNameUpper != null && x.NormalizedFirstNameUpper.Contains(req.Payload.SearchQuery)) ||
            (x.NormalizedLastNameUpper != null && x.NormalizedLastNameUpper.Contains(req.Payload.SearchQuery)));
        }

        return new()
        {
            TotalRowsCount = await q.CountAsync(),
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection,
            Response = [..await q.OrderBy(x => x.Id)
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize)
            .Select(x => new UserInfoModel()
            {
                UserId = x.Id,
                AccessFailedCount = x.AccessFailedCount,
                Email = x.Email,
                EmailConfirmed = x.EmailConfirmed,
                GivenName = x.FirstName,
                LockoutEnabled = x.LockoutEnabled,
                LockoutEnd = x.LockoutEnd,
                PhoneNumber = x.PhoneNumber,
                Surname = x.LastName,
                TelegramId = x.ChatTelegramId,
                UserName = x.UserName,
            })
            .ToListAsync()]
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>> GetUsersOfIdentity(string[] users_ids)
    {
        users_ids = [.. users_ids.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()];
        TResponseModel<UserInfoModel[]> res = new() { Response = [] };
        if (users_ids.Length == 0)
        {
            res.AddError("Пустой запрос");
            return res;
        }
        string[] find_users_ids = [.. users_ids.Where(x => x != GlobalStaticConstants.Roles.System)];
        if (find_users_ids.Length == 0)
        {
            res.Response = [.. users_ids.Select(x => UserInfoModel.BuildSystem())];
            return res;
        }

        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();
        ApplicationUser[] users = await identityContext
            .Users
            .Where(x => find_users_ids.Contains(x.Id))
            .ToArrayAsync();

        var users_roles = await identityContext
           .UserRoles
           .Where(x => find_users_ids.Contains(x.UserId))
           .Select(x => new { x.RoleId, x.UserId })
           .ToArrayAsync();

        EntryAltModel[] roles_names = users_roles.Length == 0
            ? []
            : await identityContext
                .Roles
                .Where(x => users_roles.Select(x => x.RoleId).Distinct().ToArray().Contains(x.Id))
                .Select(x => new EntryAltModel() { Id = x.Id, Name = x.Name })
                .ToArrayAsync();

        EntryAltTagModel[] claims = await identityContext
             .UserClaims
             .Where(x => find_users_ids.Contains(x.UserId) && x.ClaimType != null && x.ClaimType != "")
             .Select(x => new EntryAltTagModel() { Id = x.UserId, Name = x.ClaimType, Tag = x.ClaimValue })
             .ToArrayAsync();

        string[]? roles_for_user(string user_id)
        {
            return roles_names
                .Where(x => users_roles.Any(y => y.UserId == user_id && y.RoleId == x.Id))
                .Select(x => x.Name!)
                .Distinct()
                .ToArray();
        }

        UserInfoModel convert_user(ApplicationUser app_user)
        {
            return new()
            {
                GivenName = app_user.FirstName,
                Surname = app_user.LastName,
                UserId = app_user.Id,
                AccessFailedCount = app_user.AccessFailedCount,
                Email = app_user.Email,
                EmailConfirmed = app_user.EmailConfirmed,
                LockoutEnabled = app_user.LockoutEnabled,
                LockoutEnd = app_user.LockoutEnd,
                PhoneNumber = app_user.PhoneNumber,
                UserName = app_user.UserName,
                TelegramId = app_user.ChatTelegramId,
                Roles = [.. roles_for_user(app_user.Id)],
                Claims = [.. claims.Where(x => x.Id == app_user.Id).Select(x => new EntryAltModel() { Id = x.Id, Name = x.Name })]
            };
        }

        res.Response = users.Select(convert_user).ToArray();

        if (users_ids.Any(x => x == GlobalStaticConstants.Roles.System))
            res.Response = [.. res.Response.Union([UserInfoModel.BuildSystem()])];

        find_users_ids = [.. find_users_ids.Where(x => !res.Response.Any(y => y.UserId == x))];
        if (find_users_ids.Length != 0)
            res.AddWarning($"Некоторые пользователи (Identity) не найдены: {string.Join(",", find_users_ids)}");

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>> GetUsersIdentityByEmail(string[] users_emails)
    {
        users_emails = [.. users_emails.Where(x => MailAddress.TryCreate(x, out _)).Select(x => x.ToUpper())];
        TResponseModel<UserInfoModel[]> res = new() { Response = [] };
        if (users_emails.Length == 0)
        {
            res.AddError("Пустой запрос");
            return res;
        }

        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();
        ApplicationUser[] users = await identityContext
            .Users
            .Where(x => users_emails.Contains(x.NormalizedEmail))
            .ToArrayAsync();

        string[] find_users_ids = users.Select(x => x.Id).ToArray();
        var users_roles = await identityContext
           .UserRoles
           .Where(x => find_users_ids.Contains(x.UserId))
           .Select(x => new { x.RoleId, x.UserId })
           .ToArrayAsync();

        EntryAltModel[] roles_names = users_roles.Length == 0
            ? []
            : await identityContext
                .Roles
                .Where(x => users_roles.Select(x => x.RoleId).Distinct().ToArray().Contains(x.Id))
                .Select(x => new EntryAltModel() { Id = x.Id, Name = x.Name })
                .ToArrayAsync();

        EntryAltTagModel[] claims = await identityContext
             .UserClaims
             .Where(x => find_users_ids.Contains(x.UserId) && x.ClaimType != null && x.ClaimType != "")
             .Select(x => new EntryAltTagModel() { Id = x.UserId, Name = x.ClaimType, Tag = x.ClaimValue })
             .ToArrayAsync();

        string[]? roles_for_user(string user_id)
        {
            return roles_names
                .Where(x => users_roles.Any(y => y.UserId == user_id && y.RoleId == x.Id))
                .Select(x => x.Name!)
                .Distinct()
                .ToArray();
        }

        UserInfoModel convert_user(ApplicationUser app_user)
        {
            return new()
            {
                GivenName = app_user.FirstName,
                Surname = app_user.LastName,
                UserId = app_user.Id,
                AccessFailedCount = app_user.AccessFailedCount,
                Email = app_user.Email,
                EmailConfirmed = app_user.EmailConfirmed,
                LockoutEnabled = app_user.LockoutEnabled,
                LockoutEnd = app_user.LockoutEnd,
                PhoneNumber = app_user.PhoneNumber,
                UserName = app_user.UserName,
                TelegramId = app_user.ChatTelegramId,
                Roles = [.. roles_for_user(app_user.Id)],
                Claims = [.. claims.Where(x => x.Id == app_user.Id).Select(x => new EntryAltModel() { Id = x.Id, Name = x.Name })]
            };
        }

        res.Response = users.Select(convert_user).ToArray();

        if (users_emails.Any(x => x == GlobalStaticConstants.Roles.System))
            res.Response = [.. res.Response.Union([UserInfoModel.BuildSystem()])];

        find_users_ids = [.. find_users_ids.Where(x => !res.Response.Any(y => y.UserId == x))];
        if (find_users_ids.Length != 0)
            res.AddWarning($"Некоторые пользователи (Identity) не найдены: {string.Join(",", find_users_ids)}");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ChangeEmail(IdentityEmailTokenModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь #{req.UserId} не найден");

        string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(req.Token));

        IdentityResult result = await userManager.ChangeEmailAsync(user, req.Email, code);
        if (!result.Succeeded)
            return ResponseBaseModel.CreateError("Ошибка при смене электронной почты.");

        IdentityResult setUserNameResult = await userManager.SetUserNameAsync(user, req.Email);

        if (!setUserNameResult.Succeeded)
            return ResponseBaseModel.CreateError("Ошибка изменения имени пользователя.");

        return ResponseBaseModel.CreateSuccess("Благодарим вас за подтверждение изменения адреса электронной почты.");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateUserDetails(IdentityDetailsModel req)
    {
        req.FirstName ??= "";
        req.LastName ??= "";

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        ApplicationUser? user_db = await identityContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == req.UserId && (x.FirstName != req.FirstName || x.LastName != req.LastName || x.PhoneNumber != req.PhoneNum));

        if (user_db is null)
            return ResponseBaseModel.CreateInfo("Без изменений");

        await identityContext
            .Users
            .Where(x => x.Id == req.UserId)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.PhoneNumber, req.PhoneNum)
            .SetProperty(p => p.FirstName, req.FirstName)
            .SetProperty(p => p.NormalizedFirstNameUpper, req.FirstName.ToUpper())
            .SetProperty(p => p.LastName, req.LastName)
            .SetProperty(p => p.NormalizedLastNameUpper, req.LastName.ToUpper()));

        user_db.PhoneNumber = req.PhoneNum;
        user_db.FirstName = req.FirstName;
        user_db.NormalizedFirstNameUpper = req.FirstName.ToUpper();
        user_db.LastName = req.LastName;
        user_db.NormalizedLastNameUpper = req.LastName.ToUpper();

        await ClaimsUserFlush(user_db.Id);

        return ResponseBaseModel.CreateSuccess("First/Last names (and phone) update");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ClaimDelete(ClaimAreaIdModel req)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();

        switch (req.ClaimArea)
        {
            case ClaimAreasEnum.ForRole:
                IdentityRoleClaim<string>? claim_role_db = await identityContext.RoleClaims.FirstOrDefaultAsync(x => x.Id == req.Id);

                if (claim_role_db is null)
                    return ResponseBaseModel.CreateWarning($"Claim #{req.Id} не найден в БД");

                identityContext.RoleClaims.Remove(claim_role_db);
                break;
            case ClaimAreasEnum.ForUser:
                IdentityUserClaim<string>? claim_user_db = await identityContext.UserClaims.FirstOrDefaultAsync(x => x.Id == req.Id);

                if (claim_user_db is null)
                    return ResponseBaseModel.CreateError($"Claim #{req.Id} не найден в БД");

                identityContext.UserClaims.Remove(claim_user_db);
                break;
            default:
                throw new NotImplementedException("error {7F5317DC-EA89-47C3-BE2A-8A90838A113C}");
        }

        await identityContext.SaveChangesAsync();

        return ResponseBaseModel.CreateSuccess("Claim успешно удалён");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ClaimUpdateOrCreate(ClaimUpdateModel req)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();

        switch (req.ClaimArea)
        {
            case ClaimAreasEnum.ForRole:
                IdentityRoleClaim<string>? claim_role_db;
                if (req.ClaimUpdate.Id < 1)
                {
                    claim_role_db = new IdentityRoleClaim<string>() { RoleId = req.ClaimUpdate.OwnerId, ClaimType = req.ClaimUpdate.ClaimType, ClaimValue = req.ClaimUpdate.ClaimValue };
                    await identityContext.RoleClaims.AddAsync(claim_role_db);
                }
                else
                {
                    claim_role_db = await identityContext.RoleClaims.FirstOrDefaultAsync(x => x.RoleId == req.ClaimUpdate.OwnerId);
                    if (claim_role_db is null)
                        return ResponseBaseModel.CreateError($"Claim #{req.ClaimUpdate.OwnerId} не найден в БД");
                    else if (claim_role_db.ClaimType?.Equals(req.ClaimUpdate.ClaimType) == true && claim_role_db.ClaimValue?.Equals(req.ClaimUpdate.ClaimValue) == true)
                        return ResponseBaseModel.CreateInfo($"Claim #{req.ClaimUpdate.OwnerId} не изменён");

                    claim_role_db.ClaimType = req.ClaimUpdate.ClaimType;
                    claim_role_db.ClaimValue = req.ClaimUpdate.ClaimValue;
                    identityContext.RoleClaims.Update(claim_role_db);
                }

                break;
            case ClaimAreasEnum.ForUser:
                IdentityUserClaim<string>? claim_user_db;

                if (req.ClaimUpdate.Id < 1)
                {
                    claim_user_db = new IdentityUserClaim<string>() { UserId = req.ClaimUpdate.OwnerId, ClaimType = req.ClaimUpdate.ClaimType, ClaimValue = req.ClaimUpdate.ClaimValue };
                    await identityContext.UserClaims.AddAsync(claim_user_db);
                }
                else
                {
                    claim_user_db = await identityContext.UserClaims.FirstOrDefaultAsync(x => x.UserId == req.ClaimUpdate.OwnerId);
                    if (claim_user_db is null)
                        return ResponseBaseModel.CreateError($"Claim #{req.ClaimUpdate.OwnerId} не найден в БД");
                    else if (claim_user_db.ClaimType?.Equals(req.ClaimUpdate.ClaimType) == true && claim_user_db.ClaimValue?.Equals(req.ClaimUpdate.ClaimValue) == true)
                        return ResponseBaseModel.CreateInfo($"Claim #{req.ClaimUpdate.OwnerId} не изменён");

                    claim_user_db.ClaimType = req.ClaimUpdate.ClaimType;
                    claim_user_db.ClaimValue = req.ClaimUpdate.ClaimValue;
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
    public async Task<List<ClaimBaseModel>> GetClaims(ClaimAreaOwnerModel req)
    {
        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();

        List<ClaimBaseModel> res = req.ClaimArea switch
        {
            ClaimAreasEnum.ForRole => await identityContext.RoleClaims.Where(x => x.RoleId == req.OwnerId).Select(x => new ClaimBaseModel() { Id = x.Id, ClaimType = x.ClaimType, ClaimValue = x.ClaimValue }).ToListAsync(),
            ClaimAreasEnum.ForUser => await identityContext.UserClaims.Where(x => x.UserId == req.OwnerId).Select(x => new ClaimBaseModel() { Id = x.Id, ClaimType = x.ClaimType, ClaimValue = x.ClaimValue }).ToListAsync(),
            _ => throw new NotImplementedException("error {61909910-B126-4204-8AE6-673E11D49BCD}")
        };

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetLockUser(IdentityBooleanModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId);
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь не найден: {req.UserId}");

        await userManager.SetLockoutEndDateAsync(user, req.Set ? DateTimeOffset.MaxValue : null);
        return ResponseBaseModel.CreateSuccess($"Пользователь успешно [{user.Email}] {(req.Set ? "заблокирован" : "разблокирован")}");
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<UserInfoModel>> FindUsers(FindWithOwnedRequestModel req)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        IQueryable<ApplicationUser> q = identityContext.Users
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.OwnerId))
            q = q.Where(x => identityContext.UserRoles.Any(y => x.Id == y.UserId && req.OwnerId == y.RoleId));
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if (!string.IsNullOrWhiteSpace(req.FindQuery))
        {
            string upp_query = req.FindQuery.ToUpper();
            q = q.Where(x => EF.Functions.Like(x.NormalizedEmail, $"%{userManager.KeyNormalizer.NormalizeEmail(req.FindQuery)}%") || EF.Functions.Like(x.NormalizedFirstNameUpper, $"%{upp_query}%") || EF.Functions.Like(x.NormalizedLastNameUpper, $"%{upp_query}%") || x.Id == req.FindQuery);
        }
        int total = q.Count();
        q = q.OrderBy(x => x.UserName).Skip(req.PageNum * req.PageSize).Take(req.PageSize);
        var users = await q
            .Select(x => new
            {
                x.Id,
                x.UserName,
                x.Email,
                x.PhoneNumber,
                x.ChatTelegramId,
                x.EmailConfirmed,
                x.LockoutEnd,
                x.LockoutEnabled,
                x.AccessFailedCount,
                x.FirstName,
                x.LastName,
            })
            .ToArrayAsync();
        string[] users_ids = users.Select(x => x.Id).ToArray();
        var roles =
           await (from link in identityContext.UserRoles.Where(x => users_ids.Contains(x.UserId))
                  join role in identityContext.Roles on link.RoleId equals role.Id
                  select new { RoleName = role.Name, link.UserId }).ToArrayAsync();

        var claims =
           await (from claim in identityContext.UserClaims.Where(x => users_ids.Contains(x.UserId))
                  select new { claim.ClaimValue, claim.ClaimType, claim.UserId }).ToArrayAsync();

        return new()
        {
            Response = users.Select(x => UserInfoModel.Build(userId: x.Id, userName: x.UserName, email: x.Email, phoneNumber: x.PhoneNumber, telegramId: x.ChatTelegramId, emailConfirmed: x.EmailConfirmed, lockoutEnd: x.LockoutEnd, lockoutEnabled: x.LockoutEnabled, accessFailedCount: x.AccessFailedCount, firstName: x.FirstName, lastName: x.LastName, roles: roles.Where(y => y.UserId == x.Id).Select(z => z.RoleName).ToArray(), claims: claims.Where(o => o.UserId == x.Id).Select(q => new EntryAltModel() { Id = q.ClaimType, Name = q.ClaimValue }).ToArray())).ToList(),
            TotalRowsCount = total,
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ResetPassword(IdentityPasswordTokenModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        string msg;
        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId);
        if (user is null)
        {
            msg = $"Identity user ({nameof(req.UserId)}: `{req.UserId}`) не найден. error {{9D6C3816-7A39-424F-8EF1-B86732D46BD7}}";
            return (ApplicationUserResponseModel)ResponseBaseModel.CreateError(msg);
        }

        IdentityResult result = await userManager.ResetPasswordAsync(user, req.Token, req.Password);
        if (!result.Succeeded)
            return new()
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
    public async Task<TResponseModel<UserInfoModel>> FindByEmail(string email)
    {
        TResponseModel<UserInfoModel> res = new();
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            res.AddError($"Пользователь не найден: {email}");
            return res;
        }

        IList<Claim> claims = await userManager.GetClaimsAsync(user);

        res.Response = new()
        {
            UserId = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            AccessFailedCount = user.AccessFailedCount,
            EmailConfirmed = user.EmailConfirmed,
            LockoutEnabled = user.LockoutEnabled,
            LockoutEnd = user.LockoutEnd,
            PhoneNumber = user.PhoneNumber,
            TelegramId = user.ChatTelegramId,
            Roles = [.. (await userManager.GetRolesAsync(user))],
            Claims = claims.Select(x => new EntryAltModel() { Id = x.Type, Name = x.Value }).ToArray(),
        };

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> GenerateEmailConfirmation(SimpleUserIdentityModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IEmailSender<ApplicationUser> emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender<ApplicationUser>>();

        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user = await userManager.FindByEmailAsync(req.Email);
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь не найден: {req.Email}");

        string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        string callbackUrl = $"{req.BaseAddress}?userId={user.Id}&code={code}";
        await emailSender.SendConfirmationLinkAsync(user, req.Email, HtmlEncoder.Default.Encode(callbackUrl));
        return ResponseBaseModel.CreateSuccess($"Письмо с ссылкой подтверждением отправлено на адрес {req.Email}. Пожалуйста, проверьте электронную почту.");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ConfirmEmail(UserCodeModel req)
    {
        if (req.UserId is null || req.Code is null)
            return ResponseBaseModel.CreateError("UserId is null || Code is null. error {715DE145-87B0-48B0-9341-0A21962045BF}");
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId);
        if (user is null)
            return ResponseBaseModel.CreateError($"Ошибка загрузки пользователя с идентификатором {req.UserId}");
        else
        {
            string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(req.Code));
            IdentityResult result = await userManager.ConfirmEmailAsync(user, code);
            return result.Succeeded
                ? ResponseBaseModel.CreateSuccess("Благодарим вас за подтверждение вашего адреса электронной почты. Теперь вы можете авторизоваться!")
                : ResponseBaseModel.CreateError($"Ошибка подтверждения электронной почты: {string.Join(";", result.Errors.Select(x => $"[{x.Code}: {x.Description}]"))}");
        }
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ClaimsUserFlush(string user_id)
    {
        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();
        ApplicationUser app_user = await identityContext.Users.FirstAsync(x => x.Id == user_id);

        app_user.FirstName ??= "";
        app_user.NormalizedFirstNameUpper = app_user.FirstName.ToUpper();
        app_user.LastName ??= "";
        app_user.NormalizedLastNameUpper = app_user.LastName.ToUpper();

        TResponseModel<bool> res = new();

        string chat_tg_id = app_user.ChatTelegramId?.ToString() ?? "0";
        IdentityUserClaim<string>[] claims_db;
        int[] claims_ids;
        if (chat_tg_id != "0")
        {
            claims_db = await identityContext.UserClaims.Where(x => x.ClaimType == GlobalStaticConstants.TelegramIdClaimName && x.ClaimValue == chat_tg_id).ToArrayAsync();
            claims_ids = [.. claims_db.Where(x => x.UserId != app_user.Id).Select(x => x.Id)];
            if (claims_ids.Length != 0)
            {
                res.Response = await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() != 0 || res.Response;
            }

            if (!claims_db.Any(x => x.UserId == app_user.Id))
            {
                await identityContext.AddAsync(new IdentityUserClaim<string>()
                {
                    ClaimType = GlobalStaticConstants.TelegramIdClaimName,
                    ClaimValue = app_user.ChatTelegramId.ToString(),
                    UserId = app_user.Id,
                });
                res.Response = await identityContext.SaveChangesAsync() != 0 || res.Response;
            }
        }
        else
        {
            res.Response = await identityContext
                .UserClaims
                .Where(x => x.ClaimType == GlobalStaticConstants.TelegramIdClaimName && x.UserId == app_user.Id)
                .ExecuteDeleteAsync() != 0 || res.Response;
        }

        claims_db = await identityContext.UserClaims.Where(x => x.ClaimType == ClaimTypes.GivenName && x.UserId == app_user.Id).ToArrayAsync();
        if (string.IsNullOrWhiteSpace(app_user.FirstName))
        {
            if (claims_db.Length != 0)
            {
                claims_ids = claims_db.Select(x => x.Id).ToArray();
                res.Response = await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() != 0 || res.Response;
            }
        }
        else
        {
            IdentityUserClaim<string> fe;
            IOrderedEnumerable<IdentityUserClaim<string>> qo = claims_db.OrderBy(x => x.Id);

            if (claims_db.Length == 0)
            {
                await identityContext.AddAsync(new IdentityUserClaim<string>()
                {
                    ClaimType = ClaimTypes.GivenName,
                    ClaimValue = app_user.FirstName ?? "",
                    UserId = app_user.Id
                });
                res.Response = await identityContext.SaveChangesAsync() != 0 || res.Response;
            }
            else if (claims_db.Length > 1)
            {
                fe = qo.First();
                claims_ids = [.. qo.Skip(1).Select(x => x.Id)];
                res.Response = await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() != 0 || res.Response;
            }
            else
            {
                fe = qo.First();
                if (fe.ClaimValue != app_user.FirstName)
                {
                    res.Response = await identityContext
                                        .UserClaims
                                        .Where(x => x.Id == fe.Id)
                                        .ExecuteUpdateAsync(set => set.SetProperty(p => p.ClaimValue, app_user.FirstName)) != 0 || res.Response;
                }
            }
        }

        claims_db = await identityContext.UserClaims.Where(x => x.ClaimType == ClaimTypes.Surname && x.UserId == app_user.Id).ToArrayAsync();
        IOrderedEnumerable<IdentityUserClaim<string>> oq = claims_db.OrderBy(x => x.Id);
        if (string.IsNullOrWhiteSpace(app_user.FirstName))
        {
            if (claims_db.Length != 0)
            {
                claims_ids = claims_db.Select(x => x.Id).ToArray();
                res.Response = await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() != 0 || res.Response;
            }
        }
        else
        {
            if (claims_db.Length == 0)
            {
                await identityContext.AddAsync(new IdentityUserClaim<string>()
                {
                    ClaimType = ClaimTypes.Surname,
                    ClaimValue = app_user.LastName ?? "",
                    UserId = app_user.Id
                });
                res.Response = await identityContext.SaveChangesAsync() != 0 || res.Response;
            }
            else if (claims_db.Length > 1)
            {
                claims_ids = [.. oq.Skip(1).Select(x => x.Id)];
                res.Response = await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() != 0 || res.Response;
            }
            else if (claims_db[0].ClaimValue != app_user.FirstName)
            {
                res.Response = await identityContext
                   .UserClaims
                   .Where(x => x.Id == oq.First().Id)
                   .ExecuteUpdateAsync(set => set.SetProperty(p => p.ClaimValue, app_user.LastName)) != 0 || res.Response;
            }
        }

        claims_db = await identityContext.UserClaims.Where(x => x.ClaimType == ClaimTypes.MobilePhone && x.UserId == app_user.Id).ToArrayAsync();
        IOrderedEnumerable<IdentityUserClaim<string>> ot = claims_db.OrderBy(x => x.Id);
        if (string.IsNullOrWhiteSpace(app_user.PhoneNumber))
        {
            if (claims_db.Length != 0)
            {
                claims_ids = claims_db.Select(x => x.Id).ToArray();
                res.Response = await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() != 0 || res.Response;
            }
        }
        else
        {
            if (claims_db.Length == 0)
            {
                await identityContext.AddAsync(new IdentityUserClaim<string>()
                {
                    ClaimType = ClaimTypes.MobilePhone,
                    ClaimValue = app_user.PhoneNumber ?? "",
                    UserId = app_user.Id
                });
                res.Response = await identityContext.SaveChangesAsync() != 0 || res.Response;
            }
            else if (claims_db.Length > 1)
            {
                claims_ids = [.. ot.Skip(1).Select(x => x.Id)];
                res.Response = await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() != 0 || res.Response;
            }
            else if (claims_db[0].ClaimValue != app_user.PhoneNumber)
            {
                res.Response = await identityContext
                    .UserClaims
                    .Where(x => x.Id == ot.First().Id)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.ClaimValue, app_user.PhoneNumber)) != 0 || res.Response;
            }
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<RegistrationNewUserResponseModel> CreateNewUserEmail(string email)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using IUserStore<ApplicationUser> userStore = scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();

        IUserEmailStore<ApplicationUser> emailStore = GetEmailStore();
        ApplicationUser user = IdentityStatic.CreateInstanceUser();
        await userStore.SetUserNameAsync(user, email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);

        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        IdentityResult result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
            return new() { Messages = result.Errors.Select(x => new ResultMessage() { Text = $"[{x.Code}: {x.Description}]", TypeMessage = ResultTypesEnum.Error }).ToList() };

        return new()
        {
            RequireConfirmedAccount = userManager.Options.SignIn.RequireConfirmedAccount,
            RequireConfirmedEmail = userManager.Options.SignIn.RequireConfirmedEmail,
            RequireConfirmedPhoneNumber = userManager.Options.SignIn.RequireConfirmedPhoneNumber,
            Response = user.Id,
        };
    }

    /// <inheritdoc/>
    public async Task<RegistrationNewUserResponseModel> CreateNewUserWithPassword(RegisterNewUserPasswordModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using IUserStore<ApplicationUser> userStore = scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
        ApplicationUser user = IdentityStatic.CreateInstanceUser();

        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await userStore.SetUserNameAsync(user, req.Email, CancellationToken.None);
        IUserEmailStore<ApplicationUser> emailStore = GetEmailStore();
        await emailStore.SetEmailAsync(user, req.Email, CancellationToken.None);
        IdentityResult result = await userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
            return new() { Messages = result.Errors.Select(x => new ResultMessage() { Text = $"[{x.Code}: {x.Description}]", TypeMessage = ResultTypesEnum.Error }).ToList() };

        string userId = await userManager.GetUserIdAsync(user);
        loggerRepo.LogInformation($"User #{userId} [{req.Email}] created a new account with password.");

        string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        string callbackUrl = $"{req.BaseAddress}?userId={userId}&code={code}";

        IEmailSender<ApplicationUser> emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender<ApplicationUser>>();
        await emailSender.SendConfirmationLinkAsync(user, req.Email, System.Text.Encodings.Web.HtmlEncoder.Default.Encode(callbackUrl));

        RegistrationNewUserResponseModel res = new()
        {
            RequireConfirmedAccount = userManager.Options.SignIn.RequireConfirmedAccount,
            RequireConfirmedEmail = userManager.Options.SignIn.RequireConfirmedEmail,
            RequireConfirmedPhoneNumber = userManager.Options.SignIn.RequireConfirmedPhoneNumber,
            Response = userId
        };
        res.AddSuccess("Регистрация выполнена.");

        if (userManager.Options.SignIn.RequireConfirmedAccount)
        {
            res.AddInfo("Требуется подтверждение учетной записи.");
            res.AddWarning("Проверьте свой E-mail .");
        }

        return res;
    }

    #region telegram
    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>> GetUsersIdentityByTelegram(List<long> tg_ids)
    {
        tg_ids = [.. tg_ids.Where(x => x != 0)];
        TResponseModel<UserInfoModel[]> response = new() { Response = [] };
        if (tg_ids.Count == 0)
        {
            response.AddError("Пустой запрос");
            return response;
        }

        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();
        ApplicationUser[] users = await identityContext
            .Users
            .Where(x => tg_ids.Any(y => y == x.ChatTelegramId))
        .ToArrayAsync();

        string[] users_ids = [.. users.Select(x => x.Id)];

        TResponseModel<UserInfoModel[]> res_find_users_identity = await GetUsersOfIdentity(users_ids);
        if (!res_find_users_identity.Success())
        {
            response.AddRangeMessages(res_find_users_identity.Messages);
            return response;
        }

        if (res_find_users_identity.Response is null || res_find_users_identity.Response.Length == 0)
        {
            response.AddError("Не найдены пользователи");
            return response;
        }
        response.Response = res_find_users_identity.Response;

        tg_ids = [.. tg_ids.Where(x => !response.Response.Any(y => y.TelegramId == x))];
        if (tg_ids.Count != 0)
            response.AddInfo($"Некоторые пользователи (Telegram) не найдены: {string.Join(",", tg_ids)}");

        return response;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountDeleteAction(string userId)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user = await userManager.FindByIdAsync(userId); ;
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь #{userId} не найден");

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        TelegramJoinAccountModelDb[] act = await identityContext.TelegramJoinActions
            .Where(x => x.UserIdentityId == userId)
            .ToArrayAsync();
        if (act.Length == 0)
            return ResponseBaseModel.CreateInfo("Токена нет");
        else
        {
            identityContext.RemoveRange(act);
            int i = await identityContext.SaveChangesAsync();

            if (MailAddress.TryCreate(user.Email, out _))
                await mailRepo.SendEmailAsync(user.Email, "Удалён токен привязки Telegram к у/з", "Токен привязки аккаунта Telegram к учётной записи на сайте: удалён.");

            return ResponseBaseModel.CreateSuccess($"Токен удалён");
        }
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramAccountRemoveIdentityJoin(TelegramAccountRemoveJoinRequestIdentityModel req)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        ApplicationUser user = identityContext.Users.First(x => x.Id == req.UserId);
        long? tg_user_dump = user.ChatTelegramId;
        user.ChatTelegramId = null;
        identityContext.Update(user);

        if (MailAddress.TryCreate(user.Email, out _))
            await mailRepo.SendEmailAsync(user.Email, "Удаление привязки Telegram к учётной записи", $"Аккаунт Telegram {tg_user_dump} отключён от вашей учётной записи на сайте");

        TResponseModel<MessageComplexIdsModel> tgCall = await tgRemoteRepo.SendTextMessageTelegram(new SendTextMessageTelegramBotModel()
        {
            Message = $"Ваш Telegram аккаунт отключён от учётной записи {user.Email} с сайта {req.ClearBaseUri}",
            UserTelegramId = (await identityContext.TelegramUsers.FirstAsync(x => x.TelegramId == tg_user_dump)).TelegramId,
            From = "уведомление",
        });
        if (!tgCall.Success())
            loggerRepo.LogError(tgCall.Message());

        return ResponseBaseModel.CreateSuccess($"Пользователю {user.Email} удалена связь с TelegramId");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramJoinAccountModelDb>> TelegramJoinAccountCreate(string userId)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user = await userManager.FindByIdAsync(userId); ;
        if (user is null)
            return new TResponseModel<TelegramJoinAccountModelDb>() { Messages = ResponseBaseModel.CreateError($"Пользователь #{userId} не найден").Messages };//ResponseBaseModel.CreateError($"Пользователь #{userId} не найден");

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        IQueryable<TelegramJoinAccountModelDb> actions_del = identityContext.TelegramJoinActions
            .Where(x => x.UserIdentityId == userId);

        if (await actions_del.AnyAsync())
            identityContext.RemoveRange(actions_del);

        TelegramJoinAccountModelDb act = new()
        {
            GuidToken = Guid.NewGuid().ToString(),
            UserIdentityId = userId,
        };
        await identityContext.AddAsync(act);
        await identityContext.SaveChangesAsync();
        if (MailAddress.TryCreate(user.Email, out _))
        {
            TResponseModel<string> bot_username_res = await tgRemoteRepo.GetBotUsername();
            string? bot_username = bot_username_res.Response;
            //
            string msg = $"Создана ссылка привязки Telegram аккаунта к учётной записи сайта.<br/>";
            msg += $"Нужно подтвердить операцию через Telegram бота. Для этого нужно в TelegramBot @{bot_username} отправить токен:<br/><u><b>{act.GuidToken}</b></u><br/>Или ссылкой: <a href='https://t.me/{bot_username}?start={act.GuidToken}'>https://t.me/{bot_username}?start={act.GuidToken}</a><br/>";
            await mailRepo.SendEmailAsync(user.Email, "Статус привязки Telegram к у/з", msg);
        }

        return new() { Response = act, Messages = [new() { TypeMessage = ResultTypesEnum.Success, Text = "Токен сформирован" }] };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountConfirmTokenFromTelegram(TelegramJoinAccountConfirmModel req)
    {
        DateTime lifeTime = DateTime.UtcNow.AddMinutes(-req.TelegramJoinAccountTokenLifetimeMinutes);

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        TelegramJoinAccountModelDb? act = await identityContext.TelegramJoinActions
           .FirstOrDefaultAsync(x => x.CreatedAt > lifeTime && x.GuidToken == req.Token);
        if (act is null)
            return ResponseBaseModel.CreateError("Токен не существует");

        ApplicationUser? appUserDb = await identityContext.Users.FirstOrDefaultAsync(x => x.Id == act.UserIdentityId);
        if (appUserDb is null)
            return ResponseBaseModel.CreateError($"Пользователь (identity/{act.UserIdentityId}) для токена [{req.Token}] не найден в БД");

        //
        identityContext.Remove(act);
        await identityContext.SaveChangesAsync();
        //
        appUserDb.ChatTelegramId = req.TelegramId;
        identityContext.Update(appUserDb);
        await identityContext.SaveChangesAsync();

        await ClaimsUserFlush(appUserDb.Id);
        string msg;

        List<ApplicationUser> other_joins = await identityContext.Users
            .Where(x => x.ChatTelegramId == req.TelegramId && x.Id != appUserDb.Id)
            .ToListAsync();

        if (MailAddress.TryCreate(appUserDb.Email, out _))
        {
            msg = $"Аккаунт Telegram {req.TelegramId} подключился к учётной записи сайта воспользовавшись токеном из вашего профиля: <u><b>{act.GuidToken}</b></u>!<br/><br/>";

            msg += "Если это были не вы, то зайдите в профиль на сайте и удалите связь с Telegram.<br />";

            if (other_joins.Count != 0)
                msg += $"Другие привязки к этому Telegram аккаунту аннулируются: {string.Join("; ", other_joins.Select(x => x.Email))}";

            await mailRepo.SendEmailAsync(appUserDb.Email, "Подтверждение токена привязки Telegram к у/з", msg);
        }
        msg = "Токен успешно проверен, запрос на привязку вашего Telegram к учётной записи сформирован. Клиенту отправлено Email оповещение с информацией.";
        if (other_joins.Count != 0)
        {
            other_joins.ForEach(x => x.ChatTelegramId = null);
            identityContext.UpdateRange(other_joins);
            await identityContext.SaveChangesAsync();
        }

        TResponseModel<MessageComplexIdsModel> tgCall = await tgRemoteRepo.SendTextMessageTelegram(new SendTextMessageTelegramBotModel()
        {
            Message = $"Ваш Telegram аккаунт привязан к учётной записи '{appUserDb.Email}' сайта {req.ClearBaseUri}",
            UserTelegramId = req.TelegramId,
            From = "уведомление",
        });
        if (!tgCall.Success())
            loggerRepo.LogError(tgCall.Message());

        return ResponseBaseModel.CreateSuccess(msg);
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<TelegramUserViewModel>> FindUsersTelegram(FindRequestModel req)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        IQueryable<TelegramUserModelDb> query = identityContext.TelegramUsers
           .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.FindQuery))
        {
            string find_query = req.FindQuery.ToUpper();
            query = query.Where(x =>
            EF.Functions.Like(x.NormalizedFirstNameUpper, $"%{find_query.ToUpper()}%") ||
            (x.NormalizedUserNameUpper != null && EF.Functions.Like(x.NormalizedUserNameUpper, $"%{find_query.ToUpper()}%")) ||
            (x.NormalizedLastNameUpper != null && EF.Functions.Like(x.NormalizedLastNameUpper, $"%{find_query.ToUpper()}%")));
        }

        int total = query.Count();
        query = query.OrderBy(x => x.Id).Skip(req.PageNum * req.PageSize).Take(req.PageSize);

        TelegramUserModelDb[] users_tg = await query.ToArrayAsync();
        if (users_tg.Length == 0)
            return new() { Response = [] };

        List<long> tg_users_ids = users_tg.Select(y => y.TelegramId).ToList();

        var users_identity_data = await identityContext.Users
            .Where(x => x.ChatTelegramId.HasValue && tg_users_ids.Contains(x.ChatTelegramId.Value))
            .Select(x => new { x.Id, x.Email, x.ChatTelegramId })
            .ToArrayAsync();

        TelegramUserViewModel? identity_get(TelegramUserModelDb ctx)
        {
            var id_data = users_identity_data.FirstOrDefault(x => x.ChatTelegramId == ctx.TelegramId);

            if (id_data is null)
                return null;

            return TelegramUserViewModel.Build(ctx, id_data.Id, id_data?.Email);
        }

#pragma warning disable CS8619 // Допустимость значения NULL для ссылочных типов в значении не соответствует целевому типу.
        return new()
        {
            Response = users_tg.Select(identity_get).Where(x => x is not null).ToList(),
            TotalRowsCount = total,
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection
        };
#pragma warning restore CS8619 // Допустимость значения NULL для ссылочных типов в значении не соответствует целевому типу.
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramAccountRemoveTelegramJoin(TelegramAccountRemoveJoinRequestTelegramModel req)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        ApplicationUser? userIdentityDb = await identityContext.Users.FirstOrDefaultAsync(x => x.ChatTelegramId == req.TelegramId);
        if (userIdentityDb is null)
            return ResponseBaseModel.CreateError($"Пользователь с таким TelegramId ({req.TelegramId}) не найден в БД");

        userIdentityDb.ChatTelegramId = null;
        identityContext.Update(userIdentityDb);
        await identityContext.SaveChangesAsync();

        if (MailAddress.TryCreate(userIdentityDb.Email, out _))
        {
            TResponseModel<TelegramUserBaseModel> tg_user_dump = await GetTelegramUserCachedInfo(req.TelegramId);
            await mailRepo.SendEmailAsync(userIdentityDb.Email, "Удаление привязки Telegram к учётной записи", $"Telegram аккаунт {tg_user_dump.Response} отключён от вашей учётной записи на сайте.");
        }

        TelegramUserModelDb tg_user_info = await identityContext.TelegramUsers.FirstAsync(x => x.TelegramId == req.TelegramId);

        await identityContext
            .UserClaims
            .Where(x => x.ClaimType == GlobalStaticConstants.TelegramIdClaimName && x.ClaimValue == userIdentityDb.ChatTelegramId.ToString())
            .ExecuteDeleteAsync();

        TResponseModel<MessageComplexIdsModel> tgCall = await tgRemoteRepo.SendTextMessageTelegram(new SendTextMessageTelegramBotModel()
        {
            Message = $"Ваш Telegram аккаунт отключён от учётной записи {userIdentityDb.Email} с сайта {req.ClearBaseUri}",
            UserTelegramId = tg_user_info.TelegramId,
            From = "уведомление",
        });
        if (!tgCall.Success())
            loggerRepo.LogError(tgCall.Message());

        return ResponseBaseModel.CreateSuccess($"Пользователю {userIdentityDb.Email} удалена связь с TelegramId ${req.TelegramId}");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramUserBaseModel>> GetTelegramUserCachedInfo(long telegramId)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        TResponseModel<TelegramUserBaseModel> res = new() { Response = TelegramUserBaseModel.Build(await identityContext.TelegramUsers.FirstOrDefaultAsync(x => x.TelegramId == telegramId)) };
        if (res.Response is null)
            res.AddInfo($"Пользователь Telegram #{telegramId} не найден в кешэ БД");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateTelegramMainUserMessage(MainUserMessageModel setMainUserMessage)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        TelegramUserModelDb? user_db = await identityContext.TelegramUsers.FirstOrDefaultAsync(x => x.TelegramId == setMainUserMessage.UserId);
        if (user_db is null)
            return ResponseBaseModel.CreateError($"Пользователь Telegram #{setMainUserMessage.UserId} не найден в БД");
        if (user_db.MainTelegramMessageId == setMainUserMessage.MessageId)
            return ResponseBaseModel.CreateInfo($"Изменения {user_db} не требуются. Идентификатор `{nameof(user_db.MainTelegramMessageId)}` #{setMainUserMessage.MessageId} уже установлен");

        user_db.MainTelegramMessageId = setMainUserMessage.MessageId;
        identityContext.Update(user_db);
        await identityContext.SaveChangesAsync();

        return ResponseBaseModel.CreateSuccess($"Успешно. Пользователю {user_db} установлен/обновлён идентификатор `{nameof(user_db.MainTelegramMessageId)}` set:{setMainUserMessage.MessageId}");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramJoinAccountModelDb>> TelegramJoinAccountState(TelegramJoinAccountStateRequestModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return TResponseModel<TelegramJoinAccountModelDb>.Build(ResponseBaseModel.CreateError($"Пользователь #{req.UserId} не найден"));

        DateTime lifeTime = DateTime.UtcNow.AddMinutes(-req.TelegramJoinAccountTokenLifetimeMinutes);

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        TelegramJoinAccountModelDb? act = await identityContext.TelegramJoinActions
            .FirstOrDefaultAsync(x => x.CreatedAt > lifeTime && x.UserIdentityId == user.Id);

        if (act is null)
            return TResponseModel<TelegramJoinAccountModelDb>.Build(ResponseBaseModel.CreateWarning("Токена нет"));

        if (req.EmailNotify)
        {
            if (MailAddress.TryCreate(user.Email, out _))
            {
                string msg;
                TResponseModel<string> bot_username_res = await tgRemoteRepo.GetBotUsername();
                string? bot_username = bot_username_res.Response;

                msg = $"Существует ссылка привязки Telegram аккаунта к учётной записи сайта действительная до {act.CreatedAt.AddMinutes(req.TelegramJoinAccountTokenLifetimeMinutes)} ({DateTime.UtcNow - lifeTime}).<br/>";
                msg += $"Нужно подтвердить операцию через Telegram бота. Для этого нужно в TelegramBot @{bot_username} отправить токен:<br/><u><b>{act.GuidToken}</b></u><br/>Или ссылкой: <a href='https://t.me/{bot_username}?start={act.GuidToken}'>https://t.me/{bot_username}?start={act.GuidToken}</a><br/>";
                await mailRepo.SendEmailAsync(user.Email, "Статус привязки Telegram к у/з", msg);

                await mailRepo.SendEmailAsync(user.Email, "Статус привязки Telegram к у/з", msg);
            }
            else
                loggerRepo.LogError($"Ошибка уведомления на Email: {user.Email} - email не валидный. error {{BB9E05A4-37A3-4FBB-800B-9AED947A2B3B}}");
        }

        TResponseModel<TelegramJoinAccountModelDb> res = new() { Response = act };
        if (req.EmailNotify)
            res.AddAlert($"Проверьте свой ящик Email. Информация вам отправлена");

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<CheckTelegramUserAuthModel>> CheckTelegramUser(CheckTelegramUserHandleModel user)
    {
        TResponseModel<CheckTelegramUserAuthModel> res = new();
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        TelegramUserModelDb? tgUserDb = await identityContext.TelegramUsers.FirstOrDefaultAsync(x => x.TelegramId == user.TelegramUserId);
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if (tgUserDb is null)
        {
            using IDbContextTransaction transaction = identityContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
            ApplicationUser app_user = new()
            {
                ChatTelegramId = user.TelegramUserId,
                EmailConfirmed = true,

                Email = $"tg.{user.TelegramUserId}@{GlobalStaticConstants.FakeHost}",
                NormalizedEmail = userManager.NormalizeEmail($"tg.{user.TelegramUserId}@{GlobalStaticConstants.FakeHost}"),

                UserName = $"tg.{user.TelegramUserId}@{GlobalStaticConstants.FakeHost}",
                NormalizedUserName = userManager.NormalizeEmail($"tg.{user.TelegramUserId}@{GlobalStaticConstants.FakeHost}"),

                FirstName = user.FirstName,
                NormalizedFirstNameUpper = user.FirstName.ToUpper(),
                LastName = user.LastName,
                NormalizedLastNameUpper = user.LastName?.ToUpper(),
            };

            await identityContext.AddAsync(app_user);
            await identityContext.SaveChangesAsync();

            tgUserDb = TelegramUserModelDb.Build(user, app_user.Id);
            await identityContext.AddAsync(tgUserDb);
            await identityContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        else
        {
            tgUserDb!.FirstName = user.FirstName;
            tgUserDb.NormalizedFirstNameUpper = user.FirstName.ToUpper();

            tgUserDb.LastName = user.LastName;
            tgUserDb.NormalizedLastNameUpper = user.LastName?.ToUpper();

            tgUserDb.Username = user.Username ?? "";
            tgUserDb.NormalizedUserNameUpper = user.Username?.ToUpper();

            tgUserDb.FirstName = user.FirstName;
            tgUserDb.NormalizedFirstNameUpper = user.FirstName.ToUpper();
            tgUserDb.LastName = user.LastName;
            tgUserDb.NormalizedLastNameUpper = user.LastName?.ToUpper();

            tgUserDb.TelegramId = user.TelegramUserId;
            tgUserDb.IsBot = user.IsBot;
            identityContext.Update(tgUserDb);
            await identityContext.SaveChangesAsync();
        }

        ApplicationUser? appUserDb = await identityContext.Users.FirstOrDefaultAsync(x => x.ChatTelegramId == user.TelegramUserId);

        if (appUserDb is not null)
        {
            res.Response = new()
            {
                FirstName = tgUserDb.FirstName,
                LastName = tgUserDb.LastName,
                UserIdentityId = appUserDb.Id,
                TwoFactorEnabled = appUserDb.TwoFactorEnabled,
                UserEmail = appUserDb.Email,
                Username = tgUserDb.Username,
                TelegramId = user.TelegramUserId,
                MainTelegramMessageId = tgUserDb.MainTelegramMessageId,
                AccessFailedCount = appUserDb.AccessFailedCount,
                EmailConfirmed = appUserDb.EmailConfirmed,
                IsBot = user.IsBot,
                LockoutEnd = appUserDb.LockoutEnd,
                PhoneNumber = appUserDb.PhoneNumber,
                PhoneNumberConfirmed = appUserDb.PhoneNumberConfirmed,
                LockoutEnabled = appUserDb.LockoutEnabled,
            };

            if (tgUserDb.UserIdentityId != appUserDb.Id)
            {
                await identityContext
                    .TelegramUsers
                    .Where(x => x.Id == tgUserDb.Id)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.UserIdentityId, appUserDb.Id));
            }

            await ClaimsUserFlush(appUserDb.Id);

        }
        else
            res.AddWarning("Пользователь Telegram не связан с учётной записью на сайте");

        return res;
    }
    #endregion

    #region roles
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TryAddRolesToUser(UserRolesModel req)
    {
        req.RolesNames = req.RolesNames
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .DistinctBy(x => x.ToLower())
            .ToList();

        if (req.RolesNames.Count == 0)
            return ResponseBaseModel.CreateError("Не указаны роли для добавления");

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.FindByIdAsync(req.UserId); ;
        if (user is null)
            return ResponseBaseModel.CreateError($"Пользователь #{req.UserId} не найден");

        string[] roles_for_add_normalized = req.RolesNames.Select(r => userManager.NormalizeName(r)).ToArray();

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();

        // роли, которые есть в БД
        string?[] roles_that_are_in_db = await identityContext.Roles
            .Where(x => roles_for_add_normalized.Contains(x.NormalizedName))
            .Select(x => x.Name)
            .ToArrayAsync();

        // роли, которых не хватает в бд
        string[] roles_that_need_add_in_db = req.RolesNames
            .Where(x => !roles_that_are_in_db.Any(y => y?.Equals(x, StringComparison.OrdinalIgnoreCase) == true))
            .ToArray();

        if (roles_that_need_add_in_db.Length != 0)
        {
            loggerRepo.LogWarning($"Созданы новые роли: {JsonConvert.SerializeObject(roles_that_need_add_in_db)}");
            await identityContext
                .AddRangeAsync(roles_that_need_add_in_db.Select(r => new ApplicationRole() { Name = r, Title = r, NormalizedName = userManager.NormalizeName(r) }));
            await identityContext.SaveChangesAsync();
        }

        IList<string> user_roles = await userManager.GetRolesAsync(user);

        // роли, которые требуется добавить пользователю
        roles_that_need_add_in_db = roles_for_add_normalized
            .Where(x => !user_roles.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase) == true))
            .ToArray();

        if (roles_that_need_add_in_db.Length != 0)
        { // добавляем пользователю ролей
            loggerRepo.LogWarning($"Добавление ролей пользователю `{req.UserId}`: {JsonConvert.SerializeObject(roles_that_need_add_in_db)}");
            roles_that_need_add_in_db = await identityContext
                .Roles
                .Where(x => roles_that_need_add_in_db.Contains(x.NormalizedName))
                .Select(x => x.Id)
                .ToArrayAsync();

            await identityContext.AddRangeAsync(roles_that_need_add_in_db.Select(x => new IdentityUserRole<string>() { RoleId = x, UserId = req.UserId }));
            await identityContext.SaveChangesAsync();
        }
        return ResponseBaseModel.CreateSuccess($"Добавлено {roles_that_need_add_in_db.Length} ролей пользователю");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string[]>> SetRoleForUser(SetRoleForUserRequestModel req)
    {
        TResponseModel<string[]> res = new();
        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();

        IQueryable<ApplicationRole> q = identityContext
            .UserRoles
            .Where(x => x.UserId == req.UserIdentityId)
            .Join(identityContext.Roles, jr => jr.RoleId, r => r.Id, (jr, r) => r)
            .AsQueryable();

        ApplicationRole[] roles = await q
            .ToArrayAsync();

        ApplicationRole? role_bd;
        if (req.Command && !roles.Any(x => x.Name?.Contains(req.RoleName, StringComparison.OrdinalIgnoreCase) == true))
        {
            role_bd = await identityContext
                .Roles
                .FirstOrDefaultAsync(x => x.NormalizedName == req.RoleName.ToUpper());

            if (role_bd is null)
            {
                role_bd = new ApplicationRole()
                {
                    NormalizedName = req.RoleName.ToUpper(),
                    Name = req.RoleName,
                };
                await identityContext.AddAsync(role_bd);
                await identityContext.SaveChangesAsync();
            }
            await identityContext.AddAsync(new IdentityUserRole<string>() { RoleId = role_bd.Id, UserId = req.UserIdentityId });
            await identityContext.SaveChangesAsync();
            res.Response = [.. roles.Select(x => x.Name).Union([req.RoleName])];
            res.AddSuccess($"Включён в роль: {role_bd.Name}");
        }
        else if (!req.Command && roles.Any(x => x.Name?.Contains(req.RoleName, StringComparison.OrdinalIgnoreCase) == true))
        {
            role_bd = roles.First(x => x.Name?.Contains(req.RoleName, StringComparison.OrdinalIgnoreCase) == true);
            identityContext.Remove(role_bd);
            await identityContext.SaveChangesAsync();
            res.Response = [.. roles.Select(x => x.Name).Where(x => x?.Equals(req.RoleName, StringComparison.OrdinalIgnoreCase) != true)];
            res.AddSuccess($"Исключён из роли: {req.RoleName}");
        }
        else
        {
            res.AddInfo("Изменения не требуются");
            res.Response = [.. roles.Select(x => x.Name)];
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<RoleInfoModel>> GetRole(string role_id)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using RoleManager<ApplicationRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
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
    public async Task<TPaginationResponseModel<RoleInfoModel>> FindRoles(FindWithOwnedRequestModel req)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        IQueryable<ApplicationRole> q = identityContext.Roles
           .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.OwnerId))
            q = q.Where(x => identityContext.UserRoles.Any(y => x.Id == y.RoleId && req.OwnerId == y.UserId));
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using RoleManager<ApplicationRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        if (!string.IsNullOrWhiteSpace(req.FindQuery))
            q = q.Where(x => EF.Functions.Like(x.NormalizedName, $"%{roleManager.KeyNormalizer.NormalizeName(req.FindQuery)}%") || x.Id == req.FindQuery);

        int total = q.Count();
        q = q.OrderBy(x => x.Name).Skip(req.PageNum * req.PageSize).Take(req.PageSize);
        var roles = await
            q.Select(x => new
            {
                x.Id,
                x.Name,
                x.Title,
                UsersCount = identityContext.UserRoles.Count(z => z.RoleId == x.Id)
            })
            .ToArrayAsync();

        return new()
        {
            Response = roles.Select(x => new RoleInfoModel() { Id = x.Id, Name = x.Name, Title = x.Title, UsersCount = x.UsersCount }).ToList(),
            TotalRowsCount = total,
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CreateNewRole(string role_name)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using RoleManager<ApplicationRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
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

        return new()
        {
            Messages = ir.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}: {x.Description}]" }).ToList()
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteRole(string role_name)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using RoleManager<ApplicationRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
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

        return new()
        {
            Messages = ir.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}: {x.Description}]" }).ToList()
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteRoleFromUser(RoleEmailModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using RoleManager<ApplicationRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        ApplicationRole? role_db = await roleManager.FindByNameAsync(req.RoleName);
        if (role_db is null)
            return ResponseBaseModel.CreateError($"Роль с именем '{req.RoleName}' не найдена в БД");

        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user_db = await userManager.FindByEmailAsync(req.Email);
        if (user_db is null)
            return ResponseBaseModel.CreateError($"Пользователь `{req.Email}` не найден в БД");

        if (!await userManager.IsInRoleAsync(user_db, req.RoleName))
            return ResponseBaseModel.CreateWarning($"Роль '{req.RoleName}' у пользователя '{req.Email}' отсутствует.");

        IdentityResult ir = await userManager.RemoveFromRoleAsync(user_db, req.RoleName);

        if (ir.Succeeded)
            return ResponseBaseModel.CreateSuccess($"Пользователь '{req.Email}' исключён из роли '{req.RoleName}'");

        return new()
        {
            Messages = ir.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}: {x.Description}]" }).ToList()
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddRoleToUser(RoleEmailModel req)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using RoleManager<ApplicationRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        ApplicationRole? role_db = await roleManager.FindByNameAsync(req.RoleName);
        if (role_db is null)
            return ResponseBaseModel.CreateError($"Роль с именем '{req.RoleName}' не найдена в БД");

        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser? user_db = await userManager.FindByEmailAsync(req.Email);
        if (user_db is null)
            return ResponseBaseModel.CreateError($"Пользователь `{req.Email}` не найден в БД");

        if (await userManager.IsInRoleAsync(user_db, req.RoleName))
            return ResponseBaseModel.CreateWarning($"Роль '{req.RoleName}' у пользователя '{req.Email}' уже присутствует.");

        IdentityResult ir = await userManager.AddToRoleAsync(user_db, req.RoleName);

        if (ir.Succeeded)
            return ResponseBaseModel.CreateSuccess($"Пользователю '{req.Email}' добавлена роль '{req.RoleName}'");

        return new()
        {
            Messages = ir.Errors.Select(x => new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = $"[{x.Code}: {x.Description}]" }).ToList()
        };
    }
    #endregion

    IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        using IUserStore<ApplicationUser> userStore = scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();

        using UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("Для пользовательского интерфейса по умолчанию требуется хранилище пользователей с поддержкой электронной почты.");
        }
        return (IUserEmailStore<ApplicationUser>)userStore;
    }
}