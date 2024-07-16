////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Поле формы: Справочник/Список/Перечисление
/// </summary>
public class FieldAkaDirectoryFitModel : BaseRequiredFormFitModel
{
    /// <summary>
    /// Системное Имя : Справочник/Список/Перечисление
    /// </summary>
    public required string DirectorySystemName { get; set; }
}