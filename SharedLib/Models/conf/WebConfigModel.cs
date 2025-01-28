////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// web config
/// </summary>
public class WebConfigModel
{
    /// <inheritdoc/>
    public static readonly string Configuration = "WebConfig";

    /// <summary>
    /// BaseUri
    /// </summary>
    public string? BaseUri { get; set; }

    /// <summary>
    /// BaseUri
    /// </summary>
    public string? ClearBaseUri => string.IsNullOrWhiteSpace(BaseUri) ? string.Empty : BaseUri.EndsWith('/') ? BaseUri[..(BaseUri.Length - 1)] : BaseUri;

    /// <inheritdoc/>
    public ResponseBaseModel Update(string baseUri)
    {
        ResponseBaseModel res = new();
        if (BaseUri?.Equals(baseUri) == true)
            res.AddInfo($"Установка {nameof(BaseUri)} не требуется: значения идентичны");
        else
        {
            BaseUri = baseUri;
            res.AddSuccess($"Успешно установлен {nameof(BaseUri)}: {baseUri}");
        }

        return res;
    }
}