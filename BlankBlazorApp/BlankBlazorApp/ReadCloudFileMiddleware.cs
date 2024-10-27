////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlankBlazorApp.Properties;
using Microsoft.Extensions.Primitives;
using SharedLib;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// ReadCloudFileMiddleware
/// </summary>
public partial class ReadCloudFileMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    HttpContext? _http_context;

    /// <summary>
    /// Конвейер
    /// </summary>
    public async Task Invoke(HttpContext http_context, ISerializeStorageRemoteTransmissionService storeRepo, ILogger<ReadCloudFileMiddleware> _logger)
    {
        _http_context = http_context;
        ClaimsPrincipal user = http_context.User;

        //if (user.Identity?.IsAuthenticated != true)
        //{
        //    await http_context.Response.BodyWriter.WriteAsync(Resources.unauthorizedimage);
        //    return;
        //}

        string path = http_context.Request.Path;
        Regex rx = MyRegexDx();
        Match _match = rx.Match(path);
        if (_match.Groups.Count != 3)
        {
            await http_context.Response.BodyWriter.WriteAsync(Resources.noimage_simple);
            return;
        }
        if (!int.TryParse(_match.Groups[1].Value, out int fileId))
        {
            await http_context.Response.BodyWriter.WriteAsync(Resources.noimage_simple);
            return;
        }
        Claim? userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        TResponseModel<StorageFileResponseModel> rest = await storeRepo
            .ReadFile(new TAuthRequestModel<RequestFileReadModel>()
            {
                SenderActionUserId = userId?.Value ?? "",
                Payload = new()
                {
                    FileId = fileId,
                    TokenAccess = http_context.Request.Query.TryGetValue("token", out StringValues FileReadToken)
                        ? FileReadToken.FirstOrDefault()
                        : null
                }
            });

        if (!rest.Success() || rest.Response is null || rest.Response.Payload.Length == 0)
        {
            http_context.Response.Headers.Append(nameof(rest.Message), rest.Message());
            await http_context.Response.BodyWriter.WriteAsync(Resources.noimage_simple);
            return;
        }

        await http_context.Response.BodyWriter.WriteAsync(rest.Response.Payload);
        await _next.Invoke(_http_context);
    }

    [GeneratedRegex(@"^/(\d+)/(.*)", RegexOptions.Compiled)]
    private static partial Regex MyRegexDx();
}