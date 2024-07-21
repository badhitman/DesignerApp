////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using SharedLib;
using System.ComponentModel.DataAnnotations;

namespace CodegeneratorLib;

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
    public string TypeName { get; }

    /// <summary>
    /// Базовый путь к файлу
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string BasePath { get; }

    /// <summary>
    /// Префикс пути к файлу
    /// </summary>
    public string? PrefixPath { get; }

    /// <summary>
    /// Полный путь/имя файла типа данных (class)
    /// </summary>
    /// <param name="prefix_type_name">Префикс имени типа данных. Например: I, для объявления интерфейсов</param>
    /// <returns>Путь к элементу в архиве</returns>
    public string FullEntryName(string? prefix_type_name = null) => string.IsNullOrWhiteSpace(PrefixPath)
        ? $"{Path.Combine(BasePath, $"{prefix_type_name}{TypeName}")}.cs"
        : $"{Path.Combine(BasePath, PrefixPath, $"{prefix_type_name}{TypeName}")}.cs";
}