using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Entry type
/// </summary>
public class EntryTypeModel
{
    /// <summary>
    /// Entry type
    /// </summary>
    public EntryTypeModel(string typeName, string basePath, string? prefixPath = null)
    {
        TypeName = typeName;
        BasePath = basePath;
        PrefixPath = prefixPath;
        //
        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(this);
        if (!IsValid)
            throw new Exception($"{string.Join(";", ValidationResults.Select(x => x.ErrorMessage))};");
    }

    /// <summary>
    /// Имя файла класса (тип данных)
    /// </summary>
    [RegularExpression(GlobalStaticConstants.SYSTEM_NAME_TEMPLATE, ErrorMessage = GlobalStaticConstants.SYSTEM_NAME_TEMPLATE_MESSAGE)]
    public string TypeName { get; set; }

    /// <summary>
    /// Базовый путь к файлу
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string BasePath { get; set; }

    /// <summary>
    /// Префикс пути к файлу
    /// </summary>
    public string? PrefixPath { get; set; }

    /// <summary>
    /// Полный путь/имя файла типа данных (class)
    /// </summary>
    public string FullEntryName => string.IsNullOrWhiteSpace(PrefixPath)
        ? $"{Path.Combine(BasePath, TypeName)}.cs"
        : $"{Path.Combine(BasePath, PrefixPath, TypeName)}.cs";
}