////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Конфигурация генерации кода
/// </summary>
public class CodeGeneratorConfigModel
{
    /// <summary>
    /// Пространство имён
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [Namespace]
    public required string Namespace { get; set; }

    /// <summary>
    /// Путь размещения файлов моделей перечисления
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string EnumDirectoryPath { get; set; } = "gen_enumerations";

    /// <summary>
    /// Путь размещения файлов моделей документов
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string DocumentsMastersDbDirectoryPath { get; set; } = "gen_documents";

    /// <summary>
    /// Папка размещения инфраструктуры доступа к данным
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string AccessDataDirectoryPath { get; set; } = "gen_crud";

    /// <summary>
    /// Папка размещения файлов контроллеров
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string ControllersDirectoryPath { get; set; } = "gen_controllers";
}
