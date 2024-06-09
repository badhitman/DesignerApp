namespace SharedLib;

/// <summary>
/// Страница анкеты/опроса
/// </summary>
public class FormQuestionnairePageResponseModel : ResponseBaseModel
{
    /// <summary>
    /// Страница анкеты/опроса
    /// </summary>
    public ConstructorFormQuestionnairePageModelDB? QuestionnairePage { get; set; }
}