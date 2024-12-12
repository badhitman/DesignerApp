////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрос создания сессии
/// </summary>
public class PartUploadSessionStartRequestModel
{
    /// <summary>
    /// Размер файла
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// RemoteDirectory
    /// </summary>
    public required string RemoteDirectory { get; set; }

    /// <summary>
    /// FileName
    /// </summary>
    public required string FileName { get; set; }
}