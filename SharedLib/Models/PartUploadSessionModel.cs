////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// PartUploadSessionModel
/// </summary>
public class PartUploadSessionModel : PartUploadedBaseModel
{
    /// <summary>
    /// FilePartsMetadata
    /// </summary>
    public required List<FilePartMetadataModel> FilePartsMetadata { get; set; }

    /// <summary>
    /// RemoteDirectory
    /// </summary>
    public required string RemoteDirectory { get; set; }
}