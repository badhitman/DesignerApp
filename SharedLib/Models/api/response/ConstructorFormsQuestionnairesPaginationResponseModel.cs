////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Опросы/анкеты (с пагинацией)
/// </summary>
public class ConstructorFormsQuestionnairesPaginationResponseModel : PaginationResponseModel
{
    /// <summary>
    /// Опросы/анкеты (с пагинацией)
    /// </summary>
    public ConstructorFormsQuestionnairesPaginationResponseModel() { }

    /// <summary>
    /// Опросы/анкеты (с пагинацией)
    /// </summary>
    public ConstructorFormsQuestionnairesPaginationResponseModel(PaginationRequestModel req) { }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<ConstructorFormQuestionnaireModelDB>? Questionnaires { get; set; }
}