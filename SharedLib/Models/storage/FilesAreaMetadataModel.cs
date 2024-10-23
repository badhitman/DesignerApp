////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// AllFilesMetadataModel
/// </summary>
public class FilesAreaMetadataModel
{
    /// <summary>
    /// ApplicationName
    /// </summary>
    public required string ApplicationName { get; set; }

    /// <summary>
    /// Количество файлов
    /// </summary>
    public required int CountFiles { get; set; }

    /// <summary>
    /// Размер файлов (суммарно)
    /// </summary>
    public required long SizeFilesSum { get; set; }
}