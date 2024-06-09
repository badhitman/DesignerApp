﻿namespace SharedLib;

/// <summary>
/// Базовая модель доступа значения поля формы
/// </summary>
public class ValueFieldSessionQuestionnaireBaseModel : FieldSessionQuestionnaireBaseModel
{
    /// <summary>
    /// Группировка по номеру строки
    /// </summary>
    public uint GroupByRowNum { get; set; }

    /// <inheritdoc/>
    public bool IsSelf { get; set; } = false;
}