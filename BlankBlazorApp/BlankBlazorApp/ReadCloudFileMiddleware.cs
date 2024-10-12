////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlankBlazorApp.Properties;
using SharedLib;
using System.Text.RegularExpressions;

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
        System.Security.Claims.ClaimsPrincipal user = http_context.User;

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

        TResponseModel<StorageFileResponseModel> rest = await storeRepo.ReadFile(fileId);
        if (!rest.Success() || rest.Response is null || rest.Response.Payload.Length == 0)
        {
            await http_context.Response.BodyWriter.WriteAsync(Resources.noimage_simple);
            return;
        }

        await http_context.Response.BodyWriter.WriteAsync(rest.Response.Payload);
        await _next.Invoke(_http_context);
    }

    [GeneratedRegex(@"^/(\d+)/(.*)", RegexOptions.Compiled)]
    private static partial Regex MyRegexDx();
}