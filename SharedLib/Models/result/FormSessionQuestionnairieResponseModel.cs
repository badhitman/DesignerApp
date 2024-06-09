namespace SharedLib;

/// <summary>
/// Сессия опроса/анккеты
/// </summary>
public class FormSessionQuestionnairieResponseModel : ResponseBaseModel
{
    /// <summary>
    /// Сессия опроса/анккеты
    /// </summary>
    public ConstructorFormSessionModelDB? SessionQuestionnairie { get; set; }
}