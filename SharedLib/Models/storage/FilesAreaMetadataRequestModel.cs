////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Получить сводку (метаданные) по пространствам хранилища
/// </summary>
/// <remarks>
/// Общий размер и количество группируется по AppName
/// </remarks>
public class FilesAreaMetadataRequestModel
{
    /// <summary>
    /// ApplicationsNames - filter
    /// </summary>
    public string[]? ApplicationsNamesFilter { get; set; }
}