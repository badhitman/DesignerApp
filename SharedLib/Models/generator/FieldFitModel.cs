////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Поле формы
/// </summary>
public class FieldFitModel : BaseRequiredmFormFitModel
{
    /// <summary>
    /// Тип данных поля
    /// </summary>
    public TypesFieldsFormsEnum TypeField { get; set; }

    /// <summary>
    /// Метаданне типа значения (параметры/ограничения)
    /// </summary>
    public string? MetadataValueType { get; set; }
}