////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using IdentityLib;
using SharedLib;

namespace ServerLib;

/// <summary>
/// Сервис работы с аутентификацией пользователей
/// </summary>
public class UsersAuthenticateService(
    ILogger<UsersAuthenticateService> loggerRepo,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IStorageTransmission StorageTransmissionRepo,
    IIdentityTransmission identityRepo,
    IHttpContextAccessor httpContextAccessor,
    IOptions<UserManageConfigModel> userManageConfig) : IUsersAuthenticateService
{
    UserManageConfigModel UserConfMan => userManageConfig.Value;

    /// <inheritdoc/>
    public async Task<IdentityResultResponseModel> TwoFactorAuthenticatorSignIn(string code, bool isPersistent, bool rememberClient, string? userAlias = null)
    {
        string authenticatorCode = code.Replace(" ", string.Empty).Replace("-", string.Empty);

        if (await signInManager.GetTwoFactorAuthenticationUserAsync() is not null)
        {
            SignInResult result = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, isPersistent, rememberClient);

            return new()
            {
                IsLockedOut = result.IsLockedOut,
                IsNotAllowed = result.IsNotAllowed,
                RequiresTwoFactor = result.RequiresTwoFactor,
                Succeeded = result.Succeeded,
            };
        }
        else if (!string.IsNullOrWhiteSpace(userAlias))
        {
            TResponseModel<string> checkToken = await identityRepo.CheckToken2FA(new() { Token = authenticatorCode, UserAlias = userAlias });
            if (checkToken.Success() && !string.IsNullOrWhiteSpace(checkToken.Response))
            {
                ApplicationUser? user = await userManager.FindByIdAsync(checkToken.Response);
                if (user is null)
                {
                    return new()
                    {
                        Succeeded = false,
                        Messages = [new() { Text = $"Пользователь {checkToken.Response} не найден", TypeMessage = ResultTypesEnum.Error }]
                    };
                }

                bool _isLockedOut = default!, isEmailConfirmed = default!;
                await Task.WhenAll([
                    Task.Run(async () => { _isLockedOut = await userManager.IsLockedOutAsync(user); }), 
                    Task.Run(async () => { isEmailConfirmed = await userManager.IsEmailConfirmedAsync(user); })
                ]);

                if (_isLockedOut)
                    return new()
                    {
                        IsLockedOut = true,
                        Succeeded = false,
                        Messages = [new() { Text = "Пользователь заблокирован", TypeMessage = ResultTypesEnum.Error }]
                    };

                if (!isEmailConfirmed)
                    return new()
                    {
                        IsNotAllowed = true,
                        Succeeded = false,
                        Messages = [new() { Text = "Email пользователя не подтверждён", TypeMessage = ResultTypesEnum.Error }]
                    };

                await SignIn(checkToken.Response, isPersistent);
                return new()
                {
                    Succeeded = true,
                    Messages = [new() { TypeMessage = ResultTypesEnum.Success, Text = "Проверка пройдена успешно" }]
                };
            }
        }

        return new()
        {
            Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Ошибка" }]
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel?>> GetTwoFactorAuthenticationUser()
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
    public async Task<IdentityResultResponseModel> TwoFactorRecoveryCodeSignIn(string recoveryCode)
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

        if (result.Succeeded)
        {
            msg = $"Пользователь с идентификатором '{user.Id}' вошел в систему с кодом восстановления.";
            loggerRepo.LogInformation(msg);
            return new() { };
        }
        else if (result.IsLockedOut)
        {
            msg = $"Учетная запись пользователя #'{user.Id}' заблокирована.";
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
            msg = $"Для пользователя с идентификатором введен неверный код восстановления. '{user.Id}'";
            loggerRepo.LogWarning(msg);
            return (IdentityResultResponseModel)ResponseBaseModel.CreateError(msg);
        }
    }

    /// <inheritdoc/>
    public async Task<UserLoginInfoResponseModel> GetExternalLoginInfo(string? expectedXsrf = null)
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
    public async Task<ExternalLoginSignInResponseModel> ExternalLoginSignIn(string loginProvider, string providerKey, string? identityName, bool isPersistent = false, bool bypassTwoFactor = true)
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
    public async Task<RegistrationNewUserResponseModel> RegisterNewUser(RegisterNewUserPasswordModel req)
    {
        if (!UserConfMan.UserRegistrationIsAllowed(req.Email))
            return new() { Messages = [new() { Text = $"Ошибка регистрации {UserConfMan.DenyAuthorization?.Message}", TypeMessage = ResultTypesEnum.Error }] };

        RegistrationNewUserResponseModel regUserRes = await identityRepo.CreateNewUserWithPassword(req);
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
    public async Task<RegistrationNewUserResponseModel> ExternalRegisterNewUser(string userEmail, string baseAddress)
    {
        ExternalLoginInfo? externalLoginInfo = await signInManager.GetExternalLoginInfoAsync();
        if (externalLoginInfo == null)
            return (RegistrationNewUserResponseModel)ResponseBaseModel.CreateError("externalLoginInfo == null. error {D991FA4A-9566-4DD4-B23A-DEB497931FF5}");

        RegistrationNewUserResponseModel regUserRes = await identityRepo.CreateNewUser(userEmail);
        if (!regUserRes.Success() || string.IsNullOrWhiteSpace(regUserRes.Response))
            return regUserRes;

        ApplicationUser? user = await userManager.FindByIdAsync(regUserRes.Response);
        if (user is null)
        {
            regUserRes.AddError("Созданный пользователь не найден");
            return regUserRes;
        }

        IdentityResult result = await userManager.AddLoginAsync(user, externalLoginInfo);
        if (result.Succeeded)
        {
            loggerRepo.LogInformation("Пользователь создал учетную запись с помощью провайдера {Name}.", externalLoginInfo.LoginProvider);

            ResponseBaseModel genConfirm = await identityRepo.GenerateEmailConfirmation(new() { BaseAddress = baseAddress, Email = userEmail });
            if (!genConfirm.Success() || regUserRes.RequireConfirmedAccount == true)
            {
                regUserRes.AddRangeMessages(genConfirm.Messages);
                return regUserRes;
            }

            await signInManager.SignInAsync(user, isPersistent: false, externalLoginInfo.LoginProvider);
            return (RegistrationNewUserResponseModel)ResponseBaseModel.CreateSuccess("Вход выполнен");
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
    public async Task<ResponseBaseModel> SignIn(string userId, bool isPersistent)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return ResponseBaseModel.CreateError("Пользователь не найден");

        await signInManager.SignInAsync(user, isPersistent: false);
        return ResponseBaseModel.CreateSuccess("Пользователь авторизован");
    }

    /// <inheritdoc/>
    public async Task<SignInResultResponseModel> PasswordSignIn(string userEmail, string password, bool isPersistent)
    {
        if (!UserConfMan.UserAuthorizationIsAllowed(userEmail))
            return new() { Messages = [new() { Text = $"Ошибка авторизации {UserConfMan.DenyAuthorization?.Message}", TypeMessage = ResultTypesEnum.Error }] };

        SignInResultResponseModel res = new();
        ApplicationUser? currentAppUser = await userManager.FindByEmailAsync(userEmail);
        if (currentAppUser is null)
            return (SignInResultResponseModel)ResponseBaseModel.CreateError($"current user by email '{userEmail}' is null. error {{A19FC284-C437-4CC6-A7D2-C96FC6F6A42F}}");

        TResponseModel<bool?> globalEnable2FA = await StorageTransmissionRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.GlobalEnable2FA);
        if (globalEnable2FA.Response == true)
        {
            ResponseBaseModel chkUserPass = await identityRepo.CheckUserPassword(new() { Password = password, UserId = currentAppUser.Id });

            if (!chkUserPass.Success())
                return new() { Messages = chkUserPass.Messages };

            TResponseModel<string> otp = await identityRepo.GenerateToken2FA(currentAppUser.Id);
            res.RequiresTwoFactor = true;
            res.UserId = otp.Response;
            return res;
        }

        SignInResult sr = await signInManager.PasswordSignInAsync(userEmail, password, isPersistent, lockoutOnFailure: true);

        if (!sr.Succeeded)
        {
            if (sr.RequiresTwoFactor)
            {
                res.AddError("Error: RequiresTwoFactor");
                TResponseModel<string> otp = await identityRepo.GenerateToken2FA(currentAppUser.Id);
                return new() { RequiresTwoFactor = true, Messages = res.Messages, UserId = otp.Response };
            }

            if (sr.IsLockedOut)
                res.AddError("Ошибка: Учетная запись пользователя заблокирована.");

            if (res.Messages.Count == 0)
                res.AddError("user login error {7D55A217-6074-4988-8774-74F995F70D18}");

            return res;
        }

        ResponseBaseModel flushRes = await identityRepo.ClaimsUserFlush(currentAppUser.Id);

        if (flushRes.Success())
            await signInManager.RefreshSignInAsync(currentAppUser);

        FlushUserRolesModel? user_flush = userManageConfig.Value.UpdatesUsersRoles?.FirstOrDefault(x => x.EmailUser.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
        if (user_flush is not null)
        {
            ResponseBaseModel add_res = await identityRepo.TryAddRolesToUser(new() { RolesNames = user_flush.SetRoles, UserId = currentAppUser.Id });
            if (add_res.Success())
                await signInManager.RefreshSignInAsync(currentAppUser);
        }

        return new()
        {
            IsLockedOut = sr.IsLockedOut,
            IsNotAllowed = sr.IsNotAllowed,
            RequiresTwoFactor = sr.RequiresTwoFactor,
            Succeeded = sr.Succeeded,
            UserId = currentAppUser.Id,
        };
    }

    /// <inheritdoc/>
    public async Task SignOut()
    {
        if (httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
            await signInManager.SignOutAsync();
    }
}