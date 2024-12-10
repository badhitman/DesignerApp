////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// FilePartMetadataModel
/// </summary>
public class FilePartMetadataModel
{
    /// <summary>
    /// Номер (порядковый) порции данных
    /// </summary>
    public uint PartFileIndex { get; set; }

    /// <summary>
    /// PartFileId
    /// </summary>
    public required string PartFileId { get; set; }

    /// <summary>
    /// PartFile Position Start
    /// </summary>
    public required long PartFilePositionStart { get; set; }

    /// <summary>
    /// PartFile Position End
    /// </summary>
    public required long PartFilePositionEnd { get; set; }
}