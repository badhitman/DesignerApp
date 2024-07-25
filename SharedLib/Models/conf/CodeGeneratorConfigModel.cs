////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using static SharedLib.NamespaceAttribute;

namespace SharedLib;

/// <summary>
/// Конфигурация генерации кода
/// </summary>
public class CodeGeneratorConfigModel
{
    /// <inheritdoc/>
    public const string MessageErrorTemplateNameFolder = "Может содержать латинские буквы, цифры, дефис и знак подчёркивания. Минимум 2 символа!";

    /// <summary>
    /// Пространство имён
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [Namespace(Mode = SegmentationsNamespaceModesEnum.Any)]
    public required string Namespace { get; set; }

    /// <summary>
    /// Путь размещения файлов моделей перечисления
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.FOLDER_NAME_TEMPLATE, ErrorMessage = $"Укажите корректное имя папки размещения файлов моделей перечисления: {MessageErrorTemplateNameFolder}")]
    public string EnumDirectoryPath { get; set; } = "enumerations_generation";

    /// <summary>
    /// Путь размещения файлов моделей документов
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.FOLDER_NAME_TEMPLATE, ErrorMessage = $"Укажите корректное имя папки размещения файлов моделей документов: {MessageErrorTemplateNameFolder}")]
    public string DocumentsMastersDbDirectoryPath { get; set; } = "models_generation";

    /// <summary>
    /// Папка размещения инфраструктуры доступа к данным
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.FOLDER_NAME_TEMPLATE, ErrorMessage = $"Укажите корректное имя папки размещения файлов инфраструктуры доступа к данным: {MessageErrorTemplateNameFolder}")]
    public string AccessDataDirectoryPath { get; set; } = "crud_generation";

    /// <summary>
    /// Папка размещения Blazor UI
    /// </summary>
    /// <summary>
    /// Папка размещения инфраструктуры доступа к данным
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.FOLDER_NAME_TEMPLATE, ErrorMessage = $"Укажите корректное имя папки размещения файлов инфраструктуры доступа к данным: {MessageErrorTemplateNameFolder}")]
    public string BlazorDirectoryPath { get; set; } = "blazor_generation";

    /// <summary>
    /// Раздельные файлы: *.razor + *.razor.cs
    /// </summary>
    public bool BlazorSplitFiles { get; set; }
}