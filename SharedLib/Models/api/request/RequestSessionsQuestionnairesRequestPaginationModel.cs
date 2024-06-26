////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрос сессий (с пагинацией)
/// </summary>
public class RequestSessionsQuestionnairesRequestPaginationModel: SimplePaginationRequestModel
{
    /// <summary>
    /// 
    /// </summary>
    public int QuestionnaireId { get; set; }
}