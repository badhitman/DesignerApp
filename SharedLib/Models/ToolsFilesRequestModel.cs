////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрос метаданных файлов из директории
/// </summary>
public class ToolsFilesRequestModel
{
    /// <summary>
    /// Удалённая директория из которой запрашиваем данные
    /// </summary>
    public required string RemoteDirectory { get; set; }

    /// <summary>
    /// Рассчитывать MD5 хеш файла
    /// </summary>
    public bool CalculationHash { get; set; }
}