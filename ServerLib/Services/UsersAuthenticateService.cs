////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using IdentityLib;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedLib;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace ServerLib;

/// <summary>
/// Сервис работы с аутентификацией пользователей
/// </summary>
public class UsersAuthenticateService(ILogger<UsersAuthenticateService> loggerRepo, IUsersProfilesService usersProfilesRepo, UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore, IEmailSender<ApplicationUser> emailSender, SignInManager<ApplicationUser> signInManager, IOptions<UserManageConfigModel> userManageConfig) : IUsersAuthenticateService
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
            userName: au.UserName ?? "",
            email: au.Email,
            phoneNumber: au.PhoneNumber,
            au.TelegramId,
            au.EmailConfirmed,
            au.LockoutEnd,
            au.LockoutEnabled,
            au.AccessFailedCount)
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
            return new IdentityResultResponseModel() { };
        }
        else if (result.IsLockedOut)
        {
            msg = $"Учетная запись пользователя #'{userId}' заблокирована.";
            loggerRepo.LogError(msg);
            return new IdentityResultResponseModel()
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
    public async Task<ResponseBaseModel> ConfirmEmailAsync(string UserId, string Code)
    {
        if (UserId is null || Code is null)
            return ResponseBaseModel.CreateError("UserId is null || Code is null. error {715DE145-87B0-48B0-9341-0A21962045BF}");

        ApplicationUser? user = await userManager.FindByIdAsync(UserId);
        if (user is null)
            return ResponseBaseModel.CreateError($"Ошибка загрузки пользователя с идентификатором {UserId}");
        else
        {
            string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
            IdentityResult result = await userManager.ConfirmEmailAsync(user, code);
            return result.Succeeded
                ? ResponseBaseModel.CreateSuccess("Благодарим вас за подтверждение вашего адреса электронной почты. Теперь вы можете авторизоваться!")
                : ResponseBaseModel.CreateError($"Ошибка подтверждения электронной почты: {string.Join(";", result.Errors.Select(x => $"[{x.Code}: {x.Description}]"))}");
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
    public async Task<RegistrationNewUserResponseModel> RegisterNewUserAsync(string userEmail, string password, string baseAddress)
    {
        if (!UserConfMan.UserRegistrationIsAllowed(userEmail))
            return new RegistrationNewUserResponseModel() { Messages = [new() { Text = $"Ошибка регистрации {UserConfMan.DenyAuthorization?.Message}", TypeMessage = ResultTypesEnum.Error }] };

        ApplicationUser user = CreateUser();

        await userStore.SetUserNameAsync(user, userEmail, CancellationToken.None);
        IUserEmailStore<ApplicationUser> emailStore = GetEmailStore();
        await emailStore.SetEmailAsync(user, userEmail, CancellationToken.None);
        IdentityResult result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return new RegistrationNewUserResponseModel() { Messages = result.Errors.Select(x => new ResultMessage() { Text = $"[{x.Code}: {x.Description}]", TypeMessage = ResultTypesEnum.Error }).ToList() };

        string userId = await userManager.GetUserIdAsync(user);
        loggerRepo.LogInformation($"User #{userId} [{userEmail}] created a new account with password.");

        string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

#if DEBUG
        string code2 = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
#endif

        string callbackUrl = $"{baseAddress}?userId={userId}&code={code}";

        await emailSender.SendConfirmationLinkAsync(user, userEmail, HtmlEncoder.Default.Encode(callbackUrl));

        RegistrationNewUserResponseModel res = new()
        {
            RequireConfirmedAccount = userManager.Options.SignIn.RequireConfirmedAccount,
            RequireConfirmedEmail = userManager.Options.SignIn.RequireConfirmedEmail,
            RequireConfirmedPhoneNumber = userManager.Options.SignIn.RequireConfirmedPhoneNumber,
            Response = userId
        };

        if (userManager.Options.SignIn.RequireConfirmedAccount)
        {
            res.AddSuccess("Регистрация выполнена.");
            res.AddInfo("Требуется подтверждение учетной записи.");
            res.AddWarning("Проверьте свой E-mail .");
            return res;
        }
        await signInManager.SignInAsync(user, isPersistent: false);

        return res;
    }

    /// <inheritdoc/>
    public async Task<IdentityResultResponseModel> UserLoginAsync(string userEmail, string password, bool isPersistent)
    {
        if (!UserConfMan.UserAuthorizationIsAllowed(userEmail))
            return new IdentityResultResponseModel() { Messages = [new() { Text = $"Ошибка авторизации {UserConfMan.DenyAuthorization?.Message}", TypeMessage = ResultTypesEnum.Error }] };

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
        ApplicationUser? currentUser = await userManager.FindByEmailAsync(userEmail);
        if (currentUser is null)
            return (IdentityResultResponseModel)ResponseBaseModel.CreateError($"current user by email '{userEmail}' is null. error {{A19FC284-C437-4CC6-A7D2-C96FC6F6A42F}}");

        FlushUserRolesModel? user_flush = userManageConfig.Value.UpdatesUsersRoles?.FirstOrDefault(x => x.EmailUser.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
        if (user_flush is not null)
        {
            ResponseBaseModel add_res = await usersProfilesRepo.TryAddRolesToUser(user_flush.SetRoles, currentUser.Id);
            if (add_res.Success())
                await signInManager.RefreshSignInAsync(currentUser);
        }

        return new()
        {
            IsLockedOut = sr.IsLockedOut,
            IsNotAllowed = sr.IsNotAllowed,
            RequiresTwoFactor = sr.RequiresTwoFactor,
            Succeeded = sr.Succeeded
        };
    }

    /// <inheritdoc/>
    public async Task<RegistrationNewUserResponseModel> ExternalRegisterNewUserAsync(string userEmail, string baseAddress)
    {
        ExternalLoginInfo? externalLoginInfo = await signInManager.GetExternalLoginInfoAsync();
        if (externalLoginInfo == null)
            return (RegistrationNewUserResponseModel)ResponseBaseModel.CreateError("externalLoginInfo == null. error {D991FA4A-9566-4DD4-B23A-DEB497931FF5}");

        IUserEmailStore<ApplicationUser> emailStore = GetEmailStore();
        ApplicationUser user = CreateUser();

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
    public async Task SignOutAsync()
        => await signInManager.SignOutAsync();


    ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Не могу создать экземпляр '{nameof(ApplicationUser)}'. " +
                $"Убедитесь, что '{nameof(ApplicationUser)}' не является абстрактным классом и имеет конструктор без параметров.");
        }
    }

    IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("Для пользовательского интерфейса по умолчанию требуется хранилище пользователей с поддержкой электронной почты.");
        }
        return (IUserEmailStore<ApplicationUser>)userStore;
    }
}