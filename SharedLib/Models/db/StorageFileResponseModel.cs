////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// StorageFileResponseModel
/// </summary>
public class StorageFileResponseModel : StorageFileMiddleModel
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
