////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Конфигурация генерации кода
/// </summary>
public class CodeGeneratorConfigModel
{
    /// <summary>
    /// О проекте
    /// </summary>
    public required NameSpacedModel ProjectInfo { get; set; }

    /// <summary>
    /// Путь размещения файлов моделей перечисления
    /// </summary>
    public string EnumDirectoryPath { get; set; } = "gen_enumerations";

    /// <summary>
    /// Путь размещения файлов моделей документов
    /// </summary>
    public string DocumentsMastersDbDirectoryPath { get; set; } = "gen_documents";

    /// <summary>
    /// Папка размещения инфраструктуры доступа к данным
    /// </summary>
    public string AccessDataDirectoryPath { get; set; } = "gen_crud";

    /// <summary>
    /// Папака размещения файлов контроллеров
    /// </summary>
    public string ControllersDirectoryPath { get; set; } = "gen_controllers";
}
