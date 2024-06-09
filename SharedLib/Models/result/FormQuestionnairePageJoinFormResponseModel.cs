namespace SharedLib;

/// <summary>
/// Состав страниц анкеты/опроса (формы)
/// </summary>
public class FormQuestionnairePageJoinFormResponseModel : ResponseBaseModel
{
    /// <summary>
    /// Состав страниц анкеты/опроса (формы)
    /// </summary>
    public ConstructorFormQuestionnairePageJoinFormModelDB? QuestionnairePageJoinForm { get; set; }
}