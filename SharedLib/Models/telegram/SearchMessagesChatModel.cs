////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SearchMessagesChatModel
/// </summary>
public class SearchMessagesChatModel
{
    /// <summary>
    /// Чат
    /// </summary>
    public int ChatId { get; set; }

    /// <summary>
    /// SearchQuery
    /// </summary>
    public string? SearchQuery { get; set; }
}