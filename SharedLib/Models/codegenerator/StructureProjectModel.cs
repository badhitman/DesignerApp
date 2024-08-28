////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Структура/Состав проекта
/// </summary>
public class StructureProjectModel
{
    /// <summary>
    /// Перечисления проекта
    /// </summary>
    public required EnumFitModel[] Enumerations { get; set; }

    /// <summary>
    /// Документы проекта
    /// </summary>
    public required DocumentFitModel[] Documents { get; set; }
}