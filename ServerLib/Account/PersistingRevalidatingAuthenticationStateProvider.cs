using IdentityLib;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedLib;
using System.Diagnostics;
using System.Security.Claims;

namespace ServerLib;

/// <summary>
/// Это AuthenticationStateProvider на стороне сервера, который повторно проверяет метку безопасности для подключенного пользователя
/// каждые 30 минут, когда подключен интерактивный канал. Он также использует PersistentComponentState для передачи состояния аутентификации
/// клиенту, которое затем фиксируется на время существования приложения WebAssembly.
/// </summary>
public sealed class PersistingRevalidatingAuthenticationStateProvider(ILoggerFactory loggerFactory,
        IServiceScopeFactory scopeFactory,
        PersistentComponentState state,
        IOptions<IdentityOptions> optionsAccessor) : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{

    private readonly IdentityOptions options = optionsAccessor.Value;

    private Task<AuthenticationState>? authenticationStateTask;

    /// <inheritdoc/>
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    /// <inheritdoc/>
    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        // Get the user manager from a new scope to ensure it fetches fresh data
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        return await ValidateSecurityStampAsync(userManager, authenticationState.User);
    }

    private async Task<bool> ValidateSecurityStampAsync(UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
    {
        ApplicationUser? user = await userManager.GetUserAsync(principal);
        if (user is null)
        {
            return false;
        }
        else if (!userManager.SupportsUserSecurityStamp)
        {
            return true;
        }
        else
        {
            string? principalStamp = principal.FindFirstValue(options.ClaimsIdentity.SecurityStampClaimType);
            string userStamp = await userManager.GetSecurityStampAsync(user);
            return principalStamp == userStamp;
        }
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        authenticationStateTask = task;
    }

    private async Task OnPersistingAsync()
    {
        if (authenticationStateTask is null)
        {
            throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
        }

        AuthenticationState authenticationState = await authenticationStateTask;
        ClaimsPrincipal principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            string? givenName = principal.FindFirst(ClaimTypes.GivenName)?.Value;
            string? surName = principal.FindFirst(ClaimTypes.Surname)?.Value;
            string? userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? userName = principal.FindFirst(ClaimTypes.Name)?.Value;
            string? email = principal.FindFirst(ClaimTypes.Email)?.Value;
            string[] roles = principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();

            long? telegram_id = null;
            string? telegramIdAsString = principal.FindFirst(GlobalStaticConstants.TelegramIdClaimName)?.Value;
            if (!string.IsNullOrWhiteSpace(telegramIdAsString) && long.TryParse(telegramIdAsString, out long tgId))
                telegram_id = tgId;

            EntryAltModel[] claims = principal.FindAll((c) => c.ValueType != ClaimTypes.Role).Select(x => new EntryAltModel() { Id = x.ValueType, Name = x.Value }).ToArray();

            if (userId != null && email != null)
            {
                state.PersistAsJson(nameof(UserInfoMainModel), new UserInfoMainModel
                {
                    GivenName = givenName,
                    Surname = surName,
                    UserId = userId,
                    UserName = userName,
                    Email = email,
                    Roles = [..roles],
                    Claims = claims,
                    TelegramId = telegram_id,
                });
            }
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
        base.Dispose(disposing);
    }
}