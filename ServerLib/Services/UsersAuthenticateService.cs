////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;
using System.Text;
using IdentityLib;
using SharedLib;

namespace ServerLib;

/// <summary>
/// Сервис работы с аутентификацией пользователей
/// </summary>
public class UsersAuthenticateService(
    ILogger<UsersAuthenticateService> loggerRepo,
    IUsersProfilesService usersProfilesRepo,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IUserStore<ApplicationUser> userStore,
    IEmailSender<ApplicationUser> emailSender,
    IIdentityRemoteTransmissionService identityRepo,
    IDbContextFactory<IdentityAppDbContext> identityDbFactory,
    IOptions<UserManageConfigModel> userManageConfig) : IUsersAuthenticateService
{
    UserManageConfigModel UserConfMan => userManageConfig.Value;

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel?>> GetTwoFactorAuthenticationUserAsync()
    {
        ApplicationUser? au = await signInManager.GetTwoFactorAuthenticationUserAsync();
        if (au is null)
            return new() { Messages = ResponseBaseModel.ErrorMessage("ApplicationUser is null. error {586ED8B1-1905-472E-AB1C-69AFF0A6A191}") };

        return new()
        {
            Response = UserInfoModel.Build(
            userId: au.Id,
            userName: au.UserName,
            email: au.Email,
            phoneNumber: au.PhoneNumber,
            telegramId: au.ChatTelegramId,
            emailConfirmed: au.EmailConfirmed,
            lockoutEnd: au.LockoutEnd,
            lockoutEnabled: au.LockoutEnabled,
            accessFailedCount: au.AccessFailedCount,
            firstName: au.FirstName,
            lastName: au.LastName)
        };
    }

    /// <inheritdoc/>
    public async Task<IdentityResultResponseModel> TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
    {
        string msg;
        ApplicationUser? user = await signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user is null)
        {
            msg = "GetTwoFactorAuthenticationUser is null. error {4B4A5EDA-05E9-45BF-B283-4130394BF05E}";
            loggerRepo.LogError(msg);
            return (IdentityResultResponseModel)ResponseBaseModel.CreateError(msg);
        }
        SignInResult result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
        string userId = await userManager.GetUserIdAsync(user);
        if (result.Succeeded)
        {
            msg = $"Пользователь с идентификатором '{userId}' вошел в систему с кодом восстановления.";
            loggerRepo.LogInformation(msg);
            return new() { };
        }
        else if (result.IsLockedOut)
        {
            msg = $"Учетная запись пользователя #'{userId}' заблокирована.";
            loggerRepo.LogError(msg);
            return new()
            {
                IsLockedOut = result.IsLockedOut,
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = msg }],
                IsNotAllowed = result.IsNotAllowed,
                RequiresTwoFactor = result.RequiresTwoFactor,
                Succeeded = result.Succeeded,
            };
        }
        else
        {
            msg = $"Для пользователя с идентификатором введен неверный код восстановления. '{userId}'";
            loggerRepo.LogWarning(msg);
            return (IdentityResultResponseModel)ResponseBaseModel.CreateError(msg);
        }
    }

    /// <inheritdoc/>
    public async Task<UserLoginInfoResponseModel> GetExternalLoginInfoAsync(string? expectedXsrf = null)
    {
        ExternalLoginInfo? info = await signInManager.GetExternalLoginInfoAsync(expectedXsrf);

        return new()
        {
            IdentityName = info?.Principal.Identity?.Name,
            UserLoginInfoData = info is null ? null : new UserLoginInfoModel(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName),
            Messages = [info is null ? new() { Text = "`ExternalLoginInfo` is null. error {89E9E6CA-C9AB-4CB5-8972-681E675381F6}", TypeMessage = ResultTypesEnum.Error } : new() { Text = "login information, source and externally source principal for a user record", TypeMessage = ResultTypesEnum.Success }]
        };
    }

    /// <inheritdoc/>
    public async Task<ExternalLoginSignInResponseModel> ExternalLoginSignInAsync(string loginProvider, string providerKey, string? identityName, bool isPersistent = false, bool bypassTwoFactor = true)
    {
        // Sign in the user with this external login provider if the user already has a login.
        SignInResult result = await signInManager.ExternalLoginSignInAsync(
            loginProvider,
            providerKey,
            isPersistent: isPersistent,
            bypassTwoFactor: bypassTwoFactor);

        ExternalLoginSignInResponseModel res = new()
        {
            IsLockedOut = result.IsLockedOut,
            IsNotAllowed = result.IsNotAllowed,
            RequiresTwoFactor = result.RequiresTwoFactor,
            Succeeded = result.Succeeded
        };
        if (result.Succeeded)
        {
            loggerRepo.LogInformation(
                "{Name} logged in with {LoginProvider} provider.",
                identityName,
                loginProvider);
            return res;
        }
        else if (result.IsLockedOut)
        {
            return res;
        }

        ExternalLoginInfo? externalLoginInfo = await signInManager.GetExternalLoginInfoAsync();
        if (externalLoginInfo is null)
        {
            res.AddError("Ошибка загрузки внешних данных для входа.");
            return res;
        }

        if (externalLoginInfo.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
        {
            res.Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
        }
        return res;
    }

    /// <inheritdoc/>
    public async Task<RegistrationNewUserResponseModel> RegisterNewUserAsync(RegisterNewUserPasswordModel req)
    {
        if (!UserConfMan.UserRegistrationIsAllowed(req.Email))
            return new() { Messages = [new() { Text = $"Ошибка регистрации {UserConfMan.DenyAuthorization?.Message}", TypeMessage = ResultTypesEnum.Error }] };

        RegistrationNewUserResponseModel regUserRes = await identityRepo.CreateNewUser(req);
        if (!regUserRes.Success() || string.IsNullOrWhiteSpace(regUserRes.Response) || regUserRes.RequireConfirmedEmail == true)
            return regUserRes;

        ApplicationUser? user = await userManager.FindByIdAsync(regUserRes.Response);
        if (user is null)
        {
            regUserRes.AddError("Созданный пользователь не найден");
            return regUserRes;
        }

        await signInManager.SignInAsync(user, isPersistent: false);
        return regUserRes;
    }

    /// <inheritdoc/>
    public async Task<RegistrationNewUserResponseModel> ExternalRegisterNewUserAsync(string userEmail, string baseAddress)
    {
        ExternalLoginInfo? externalLoginInfo = await signInManager.GetExternalLoginInfoAsync();
        if (externalLoginInfo == null)
            return (RegistrationNewUserResponseModel)ResponseBaseModel.CreateError("externalLoginInfo == null. error {D991FA4A-9566-4DD4-B23A-DEB497931FF5}");


        IUserEmailStore<ApplicationUser> emailStore = GetEmailStore();
        ApplicationUser user = IdentityStatic.CreateInstanceUser();
        await userStore.SetUserNameAsync(user, userEmail, CancellationToken.None);
        await emailStore.SetEmailAsync(user, userEmail, CancellationToken.None);

        IdentityResult result = await userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            result = await userManager.AddLoginAsync(user, externalLoginInfo);
            if (result.Succeeded)
            {
                loggerRepo.LogInformation("Пользователь создал учетную запись с помощью провайдера {Name}.", externalLoginInfo.LoginProvider);

                string userId = await userManager.GetUserIdAsync(user);
                string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                string callbackUrl = $"{baseAddress}?userId={userId}&code={code}";

                await emailSender.SendConfirmationLinkAsync(user, userEmail, HtmlEncoder.Default.Encode(callbackUrl));

                if (userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return new() { RequireConfirmedAccount = true };
                }

                await signInManager.SignInAsync(user, isPersistent: false, externalLoginInfo.LoginProvider);
                return (RegistrationNewUserResponseModel)ResponseBaseModel.CreateSuccess("Вход выполнен");
            }
        }

        return new()
        {
            Messages = result.Errors.Select(x => new ResultMessage()
            {
                TypeMessage = ResultTypesEnum.Error,
                Text = $"[{x.Code}: {x.Description}]"
            }).ToList()
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SignInAsync(string userId, bool isPersistent)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(userId);

        if (user is null)
            return ResponseBaseModel.CreateError("Пользователь не найден");

        await signInManager.SignInAsync(user, isPersistent: false);
        return ResponseBaseModel.CreateSuccess("Пользователь авторизован");
    }

    /// <inheritdoc/>
    public async Task<IdentityResultResponseModel> PasswordSignInAsync(string userEmail, string password, bool isPersistent)
    {
        if (!UserConfMan.UserAuthorizationIsAllowed(userEmail))
            return new() { Messages = [new() { Text = $"Ошибка авторизации {UserConfMan.DenyAuthorization?.Message}", TypeMessage = ResultTypesEnum.Error }] };

        SignInResult sr = await signInManager.PasswordSignInAsync(userEmail, password, isPersistent, lockoutOnFailure: true);

        IdentityResultResponseModel res = new();
        if (!sr.Succeeded)
        {
            if (sr.RequiresTwoFactor)
                res.AddError("Error: RequiresTwoFactor");

            if (sr.IsLockedOut)
                res.AddError("Ошибка: Учетная запись пользователя заблокирована.");
            else
                res.AddError("Ошибка: Неверные логин/пароль (либо учётная запись неактивна).");

            if (res.Messages.Count == 0)
                res.AddError("user login error {7D55A217-6074-4988-8774-74F995F70D18}");

            return res;
        }
        ApplicationUser? currentAppUser = await userManager.FindByEmailAsync(userEmail);

        if (currentAppUser is null)
            return (IdentityResultResponseModel)ResponseBaseModel.CreateError($"current user by email '{userEmail}' is null. error {{A19FC284-C437-4CC6-A7D2-C96FC6F6A42F}}");

        ResponseBaseModel flushRes = await identityRepo.ClaimsUserFlush(currentAppUser.Id);

        if (flushRes.Success())
            await signInManager.RefreshSignInAsync(currentAppUser);

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();

        FlushUserRolesModel? user_flush = userManageConfig.Value.UpdatesUsersRoles?.FirstOrDefault(x => x.EmailUser.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
        if (user_flush is not null)
        {
            ResponseBaseModel add_res = await usersProfilesRepo.TryAddRolesToUser(user_flush.SetRoles, currentAppUser.Id);
            if (add_res.Success())
                await signInManager.RefreshSignInAsync(currentAppUser);
        }

        return new()
        {
            IsLockedOut = sr.IsLockedOut,
            IsNotAllowed = sr.IsNotAllowed,
            RequiresTwoFactor = sr.RequiresTwoFactor,
            Succeeded = sr.Succeeded,
        };
    }

    /// <inheritdoc/>
    public async Task SignOutAsync()
        => await signInManager.SignOutAsync();


    IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("Для пользовательского интерфейса по умолчанию требуется хранилище пользователей с поддержкой электронной почты.");
        }
        return (IUserEmailStore<ApplicationUser>)userStore;
    }
}