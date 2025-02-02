using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;
using SharedLib;
using Microsoft.Extensions.Options;

namespace ApiRestService;

/// <summary>
/// Авторизация по ролям
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class RolesAuthorizationFilter(IOptions<RestApiConfigBaseModel> _conf, string? Roles = null) : Attribute, IAuthorizationFilter
{
    HttpContext _http_context = default!;
    static ReadOnlySpan<char> Separator => [' ', ',', '\t', '\n', '\r'];

    /// <inheritdoc/>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        _http_context = context.HttpContext;
        string token = ReadTokenFromRequest();
        ExpressUserPermissionModel? perm = _conf.Value?.Permissions?.FirstOrDefault(x => x.Secret?.Equals(token, StringComparison.OrdinalIgnoreCase) == true);

        if (perm is null)
        {
            context.Result = new ForbidResult();
            return;
        }

        string user_name = perm.User ?? $"for user not set name (key: {perm.Secret})";
        context.HttpContext.Response.Headers.Append(nameof(perm.User), user_name);

        string[] roles = Roles?.Split(Separator).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray() ?? [];
        if (roles.Length != 0 && perm.Roles?.Any(x => roles.Any(y => y.Equals(x.ToString(), StringComparison.OrdinalIgnoreCase) || y.Equals(x.DescriptionInfo(), StringComparison.OrdinalIgnoreCase))) != true)
            context.Result = new ForbidResult();
    }

    /// <inheritdoc/>
    public string ReadTokenFromRequest()
    {
        if (_http_context is null)
            return "";

        if (_http_context.Request.Headers.TryGetValue(_conf.Value!.TokenAccessHeaderName, out StringValues tok))
            return tok.FirstOrDefault() ?? string.Empty;
        if (_http_context.Request.Query.Any(x => x.Key.Equals(_conf.Value!.TokenAccessHeaderName)))
            return _http_context.Request.Query[_conf.Value!.TokenAccessHeaderName]!;

        return "";
    }
}