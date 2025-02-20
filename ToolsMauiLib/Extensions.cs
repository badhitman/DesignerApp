////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace ToolsMauiLib;

public static class Extensions
{

    /// <summary>
    /// NormalizedUri
    /// </summary>
    public static string NormalizedUriEnd(this string cli)=> cli.EndsWith('/') ? cli : $"{cli}/";

    /*
    /// <summary>
    /// Auth
    /// </summary>
    public static HttpClient Auth(this HttpClient cli, ApiRestConfigModelDB ApiConnect)
    {
        cli.BaseAddress = string.IsNullOrWhiteSpace(ApiConnect.AddressBaseUri)
            ? null
            : new(ApiConnect.AddressBaseUri);

        if (!string.IsNullOrWhiteSpace(ApiConnect.HeaderName))
            cli.DefaultRequestHeaders.Add(ApiConnect.HeaderName, ApiConnect.TokenAccess);

        return cli;
    }*/
}