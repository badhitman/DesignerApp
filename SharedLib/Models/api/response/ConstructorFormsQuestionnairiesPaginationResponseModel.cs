namespace SharedLib;

/// <summary>
/// Опросы/анкеты (с пагинацией)
/// </summary>
public class ConstructorFormsQuestionnairiesPaginationResponseModel : PaginationResponseModel
{
    /// <summary>
    /// Опросы/анкеты (с пагинацией)
    /// </summary>
    public ConstructorFormsQuestionnairiesPaginationResponseModel() { }

    /// <summary>
    /// Опросы/анкеты (с пагинацией)
    /// </summary>
    public ConstructorFormsQuestionnairiesPaginationResponseModel(PaginationRequestModel req) { }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<ConstructorFormQuestionnaireModelDB>? Questionnairies { get; set; }
}