using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using SharedLib;

namespace ApiRestService;

/// <summary>
/// Авторизация по ролям
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class RolesAuthorizationFilter(IOptions<RestApiConfigBaseModel> _conf, ExpressUserPermissionModel permissionUser, string? Roles = null) : Attribute, IAuthorizationFilter
{
    static ReadOnlySpan<char> Separator => [' ', ',', '\t', '\n', '\r'];
    HttpContext _http_context = default!;

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

        permissionUser.Update(perm);
        //string user_name = perm.User ?? $"for user not set name (key: {perm.Secret})";
        //context.HttpContext.Response.Headers.Append(nameof(perm.User), user_name);

        string[] roles = Roles?.Split(Separator).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray() ?? [];
        if (roles.Length != 0 && perm.Roles?.Any(x => roles.Any(y => y.Equals(x.ToString(), StringComparison.OrdinalIgnoreCase))) != true)
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