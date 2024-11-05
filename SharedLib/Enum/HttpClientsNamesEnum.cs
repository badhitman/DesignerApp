////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Имена HTTP клиентов для IHttpClientFactory (например: IHttpClientFactory.CreateClient(HttpClientsNamesEnum.Insight.ToString()))
/// </summary>
public enum HttpClientsNamesEnum
{
    /// <summary>
    /// Default
    /// </summary>
    Default,

    /// <summary>
    /// Tools
    /// </summary>
    Tools,
}