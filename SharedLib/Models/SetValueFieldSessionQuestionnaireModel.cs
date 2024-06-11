﻿namespace SharedLib;

/// <summary>
/// Установка значения поля формы
/// </summary>
public class SetValueFieldSessionQuestionnaireModel : ValueFieldSessionQuestionnaireBaseModel
{
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

    internal static SetValueFieldSessionQuestionnaireModel Build(string fieldValue, string fieldName, string description, uint groupByRowNum)
    {
        return new()
        {
            FieldValue = fieldValue,
            NameField = fieldName,
            Description = description,
            GroupByRowNum = groupByRowNum
        };
    }
}