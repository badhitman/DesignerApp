////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Поле формы: Справочник/Список/Перечисление
/// </summary>
public class FieldAkaDirectoryFitModel : BaseRequiredFormFitModel
{
    /// <summary>
    /// Системное Имя : Справочник/Список/Перечисление
    /// </summary>
    public required string DirectorySystemName { get; set; }


    /// <summary>
    /// Множественный выбор
    /// </summary>
    public required bool IsMultiSelect { get; set; }
}