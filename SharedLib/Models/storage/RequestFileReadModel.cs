////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// RequestFileReadModel
/// </summary>
public class RequestFileReadModel
{
    /// <summary>
    /// FileId
    /// </summary>
    public int FileId { get; set; }

    /// <summary>
    /// File-read TokenAccess
    /// </summary>
    public string? TokenAccess { get; set; }
}