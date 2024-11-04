////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SimpleBaseRequestModel
/// </summary>
public class SimpleBaseRequestModel
{
    /// <summary>
    /// Строка поиска
    /// </summary>
    public string? SearchQuery { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    public int ProjectId { get; set; }
}