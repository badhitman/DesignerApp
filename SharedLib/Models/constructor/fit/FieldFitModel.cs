////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using SharedLib;

namespace SharedLib;

/// <summary>
/// Поле формы
/// </summary>
public class FieldFitModel : BaseRequiredFormFitModel
{
    /// <summary>
    /// Тип данных поля
    /// </summary>
    public TypesFieldsFormsEnum TypeField { get; set; }

    /// <summary>
    /// Метаданне типа значения (параметры/ограничения)
    /// </summary>
    public string? MetadataValueType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string TypeData => TypeField switch
    {
        TypesFieldsFormsEnum.Text or TypesFieldsFormsEnum.Password => "string",
        TypesFieldsFormsEnum.Int => "int",
        TypesFieldsFormsEnum.Double => "double",
        TypesFieldsFormsEnum.Bool => "bool",
        TypesFieldsFormsEnum.Date => "DateOnly",
        TypesFieldsFormsEnum.Time => "TimeOnly",
        TypesFieldsFormsEnum.DateTime => "DateTime",
        _ => throw new NotImplementedException()
    };
}