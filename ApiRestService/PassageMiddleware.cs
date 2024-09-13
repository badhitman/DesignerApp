////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Security.Principal;
using SharedLib;

namespace ApiRestService;

/// <summary>
/// Обработчик сессии
/// </summary>
public class PassageMiddleware
{
    private readonly RequestDelegate _next;
    HttpContext? _http_context;
    RestApiConfigBaseModel? _conf;

    /// <summary>
    /// Конструктор
    /// </summary>
    public PassageMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Конвеер
    /// </summary>
    public async Task Invoke(HttpContext http_context, ILogger<PassageMiddleware> _logger, IOptions<RestApiConfigBaseModel> conf)
    {
        _http_context = http_context;
        _conf = conf.Value;
        try
        {
            await InitSession();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error {nameof(PassageMiddleware)}");
        }

        await _next.Invoke(_http_context);
    }

    /// <summary>
    /// Чтение сессии из заголовков запроса
    /// </summary>
    public async Task InitSession()
    {
        if (_http_context is null)
            return;

        string token = ReadTokenFromRequest();

        ExpressUserPermissionModel? perm = _conf?.Permissions?.FirstOrDefault(x => x.Secret?.Equals(token, StringComparison.OrdinalIgnoreCase) == true);

        if (perm is null)
        {
            if (_http_context.User.Identity?.IsAuthenticated == true)
                await _http_context.SignOutAsync();

            return;
        }
        string user_name = perm.User ?? $"for user not set name (key: {perm.Secret})";
        GenericIdentity gi = new(user_name);
        gi.AddClaim(new Claim(ClaimTypes.Sid, token));
        GenericPrincipal princ = new(gi, perm.Roles?.Select(x => x.ToString()).ToArray());

        string[] current_session_roles = _http_context.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();
        string[] current_permission_roles = perm.Roles?.Any() == true
            ? perm.Roles.Select(x => x.ToString()).ToArray()
            : Array.Empty<string>();

        string? curr_sid = _http_context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;

        if (curr_sid != perm.Secret || _http_context.User?.Identity?.Name != princ.Identity.Name || _http_context.User?.Identity?.IsAuthenticated != true || current_permission_roles.Any(x => !current_session_roles.Contains(x)) || current_session_roles.Any(x => !current_permission_roles.Contains(x)))
        {
            await _http_context.SignOutAsync();
            _http_context.User = princ;
            await _http_context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, princ);
        }
    }

    /// <inheritdoc/>
    public string ReadTokenFromRequest()
    {
        if (_http_context is null)
            return "";

        if (_http_context.Request.Headers.TryGetValue("token_access", out StringValues tok))
            return tok.FirstOrDefault() ?? string.Empty;
        if (_http_context.Request.Query.Any(x => x.Key.Equals("token_access")))
            return _http_context.Request.Query["token_access"]!;

        return "";
    }
}