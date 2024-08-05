using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SharedLib;
using System.Security.Claims;

namespace BlazorClientLib;

/// <summary>
/// This is a client-side AuthenticationStateProvider that determines the user's authentication state by
/// looking for data persisted in the page when it was rendered on the server. This authentication state will
/// be fixed for the lifetime of the WebAssembly application. So, if the user needs to log in or out, a full
/// page reload is required.
///
/// This only provides a user name and email for display purposes. It does not actually include any tokens
/// that authenticate to the server when making subsequent requests. That works separately using a
/// cookie that will be included on HttpClient requests to the server.
/// </summary>
public class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;
    /// <summary>
    /// This is a client-side AuthenticationStateProvider that determines the user's authentication state by
    /// looking for data persisted in the page when it was rendered on the server. This authentication state will
    /// be fixed for the lifetime of the WebAssembly application. So, if the user needs to log in or out, a full
    /// page reload is required.
    ///
    /// This only provides a user name and email for display purposes. It does not actually include any tokens
    /// that authenticate to the server when making subsequent requests. That works separately using a
    /// cookie that will be included on HttpClient requests to the server.
    /// </summary>
    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson(nameof(UserInfoMainModel), out UserInfoMainModel? userInfo) || userInfo is null)
            return;

        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
            new Claim(ClaimTypes.Name, userInfo.UserName ?? ""),
            new Claim(ClaimTypes.Email, userInfo.Email ?? "")
            ];

        if (userInfo.Roles is not null && userInfo.Roles.Length != 0)
            claims.AddRange(userInfo.Roles.Select(x => new Claim(ClaimTypes.Role, x)));

        if (userInfo.Claims is not null && userInfo.Claims.Length != 0)
            claims.AddRange(userInfo.Claims.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(x => new Claim(x.Id, x.Name!)));

        authenticationStateTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                authenticationType: nameof(PersistentAuthenticationStateProvider)))));
    }

    /// <inheritdoc/>
    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;
}