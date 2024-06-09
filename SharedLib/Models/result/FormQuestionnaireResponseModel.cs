namespace SharedLib;

/// <summary>
/// Получить анкету/опрос
/// </summary>
public class FormQuestionnaireResponseModel : ResponseBaseModel
{
    /// <summary>
    /// Анкета/опрос
    /// </summary>
    public ConstructorFormQuestionnaireModelDB? Questionnaire { get; set; }
}