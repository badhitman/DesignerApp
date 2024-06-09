namespace SharedLib;

/// <summary>
/// Сессия опроса/анккеты
/// </summary>
public class FormSessionQuestionnaireResponseModel : ResponseBaseModel
{
    /// <summary>
    /// Сессия опроса/анккеты
    /// </summary>
    public ConstructorFormSessionModelDB? SessionQuestionnaire { get; set; }
}