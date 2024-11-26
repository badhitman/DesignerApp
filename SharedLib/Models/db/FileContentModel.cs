////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// FileContentModel
/// </summary>
public class FileContentModel : StorageFileMiddleModel
{
    /// <summary>
    /// Payload
    /// </summary>
    public required byte[] Payload { get; set; }

    /// <summary>
    /// ContentType
    /// </summary>
    public required string? ContentType { get; set; }
}
