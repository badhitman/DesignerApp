////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// StorageImageMetadataModel
/// </summary>
public class StorageImageMetadataModel : StorageMetadataModel
{
    /// <summary>
    /// FileName
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// ContentType
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Referer
    /// </summary>
    public string? Referrer { get; set; }

    /// <summary>
    /// Payload
    /// </summary>
    public required byte[] Payload { get; set; }

    /// <summary>
    /// AuthorUserIdentity
    /// </summary>
    public required string AuthorUserIdentity { get; set; }
}