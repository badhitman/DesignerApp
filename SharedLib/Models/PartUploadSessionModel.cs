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
    /// FullSizeParts
    /// </summary>
    public long FullSizeParts => FilePartsMetadata.Sum(x => x.PartFileSize);

    /// <summary>
    /// RemoteDirectory
    /// </summary>
    public required string RemoteDirectory { get; set; }

    /// <summary>
    /// FileName
    /// </summary>
    public required string FileName { get; set; }
}