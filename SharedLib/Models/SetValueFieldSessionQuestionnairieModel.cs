namespace SharedLib;

/// <summary>
/// Установка значения поля формы
/// </summary>
public class SetValueFieldSessionQuestionnairieModel : ValueFieldSessionQuestionnaireBaseModel
{
    /// <summary>
    /// Имя поля
    /// </summary>
    public string NameField { get; set; } = default!;

    /// <summary>
    /// Значение поля
    /// </summary>
    public string? FieldValue { get; set; }

    /// <summary>
    /// Описание 
    /// </summary>
    public string? Description { get; set; }
}