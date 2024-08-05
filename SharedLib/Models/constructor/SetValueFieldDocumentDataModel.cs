////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Установка значения поля формы
/// </summary>
public class SetValueFieldDocumentDataModel : ValueFieldSessionDocumentDataBaseModel
{
    /// <summary>
    /// Версия проекта (дата/время последнего изменения)
    /// </summary>
    public required DateTime ProjectVersionStamp { get; set; }

    /// <summary>
    /// Имя поля
    /// </summary>
    public required string NameField { get; set; }

    /// <summary>
    /// Значение поля
    /// </summary>
    public string? FieldValue { get; set; }

    /// <summary>
    /// Описание 
    /// </summary>
    public string? Description { get; set; }

    internal static SetValueFieldDocumentDataModel Build(DateTime projectVersionStamp, string fieldValue, string fieldName, string description, uint groupByRowNum)
    {
        return new()
        {
            FieldValue = fieldValue,
            NameField = fieldName,
            Description = description,
            GroupByRowNum = groupByRowNum,
            ProjectVersionStamp = projectVersionStamp
        };
    }
}