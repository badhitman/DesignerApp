////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using SharedLib.Models;

namespace SharedLib;

/// <summary>
/// Структура/Состав проекта
/// </summary>
public class StructureProjectModel
{
    /// <summary>
    /// Перечисления проекта
    /// </summary>
    public required IEnumerable<EnumFitModel> Enums { get; set; }

    /// <summary>
    /// Документы проекта
    /// </summary>
    public required IEnumerable<DocumentFitModel> Documents { get; set; }
}
