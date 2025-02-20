////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace ToolsMauiLib;

public static class Extensions
{

    /// <summary>
    /// NormalizedUri
    /// </summary>
    public static string NormalizedUriEnd(this string cli)=> cli.EndsWith('/') ? cli : $"{cli}/";
}