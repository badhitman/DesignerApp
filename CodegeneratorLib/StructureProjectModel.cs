////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Структура/Состав проекта
/// </summary>
public class StructureProjectModel
{
    /// <summary>
    /// Перечисления проекта
    /// </summary>
    public required EnumFitModel[] Enums { get; set; }

    /// <summary>
    /// Документы проекта
    /// </summary>
    public required DocumentFitModel[] Documents { get; set; }
}
