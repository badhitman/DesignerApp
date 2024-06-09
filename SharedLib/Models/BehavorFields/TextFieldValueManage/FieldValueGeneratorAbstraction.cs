namespace SharedLib;

/// <summary>
/// Генератор значений для выбора клиентом в специализированном контроле
/// </summary>
public abstract class FieldValueGeneratorAbstraction : DeclarationAbstraction
{
    /// <summary>
    /// модель запроса (для примера/схемы пользователю)
    /// </summary>
    public object RequestModel = new();

    /// <summary>
    /// Элементы, полученые от генератора
    /// </summary>
    public abstract SimpleStringArrayResponseModel GetListElements(ConstructorFieldFormModelDB field, ConstructorFormSessionModelDB session_Questionnaire, ConstructorFormQuestionnairePageJoinFormModelDB? page_join_form = null, uint row_num = 0);
}