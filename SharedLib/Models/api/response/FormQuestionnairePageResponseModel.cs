////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Страница анкеты/опроса
/// </summary>
public class FormQuestionnairePageResponseModel : ResponseBaseModel
{
    /// <summary>
    /// Страница анкеты/опроса
    /// </summary>
    public TabOfDocumentSchemeConstructorModelDB? QuestionnairePage { get; set; }
}