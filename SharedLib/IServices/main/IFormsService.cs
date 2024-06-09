namespace SharedLib;

/// <summary>
/// Forms служба
/// </summary>
public interface IFormsService
{
    #region public
    /// <summary>
    /// Получить сессию
    /// </summary>
    public Task<FormSessionQuestionnaireResponseModel> GetSessionQuestionnaire(string guid_session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Установить значение свойства сессии
    /// </summary>
    public Task<FormSessionQuestionnaireResponseModel> SetValueFieldSessionQuestionnaire(SetValueFieldSessionQuestionnaireModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить опрос на проверку (от клиента)
    /// </summary>
    public Task<ResponseBaseModel> SetDoneSessionQuestionnaire(string token_session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить набор значений сессии опроса/анкеты по номеру строки [GroupByRowNum].
    /// Если индексниже нуля - удаляютс все значения для указанной JoinForm (полная очистка таблицы или очистка всех значений всех поллей стандартной формы)
    /// </summary>
    public Task<ResponseBaseModel> DeleteValuesFieldsByGroupSessionQuestionnaireByRowNum(ValueFieldSessionQuestionnaireBaseModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить новую строку в таблицу значений
    /// </summary>
    /// <returns>Номер п/п (начиная с 1) созданной строки</returns>
    public Task<CreateObjectOfIntKeyResponseModel> AddRowToTable(FieldSessionQuestionnaireBaseModel req, CancellationToken cancellationToken = default);
    #endregion

    #region сессии опросов/анкет
    /// <summary>
    /// Установить статус сессии (от менеджера)
    /// </summary>
    public Task<ResponseBaseModel> SetStatusSessionQuestionnaire(int id_session, SessionsStatusesEnum status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить сессию
    /// </summary>
    public Task<FormSessionQuestionnaireResponseModel> GetSessionQuestionnaire(int id_session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить (или создать) сессию опроса/анкеты
    /// </summary>
    public Task<FormSessionQuestionnaireResponseModel> UpdateOrCreateSessionQuestionnaire(ConstructorFormSessionModelDB session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запросить порцию сессий (с пагинацией)
    /// </summary>
    public Task<ConstructorFormsSessionsPaginationResponseModel> RequestSessionsQuestionnaires(RequestSessionsQuestionnairesRequestPaginationModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Найти порцию сессий по имени поля (с пагинацией)
    /// </summary>
    public Task<EntriesDictResponseModel> FindSessionsQuestionnairesByFormFieldName(FormFieldModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить значения (введёные в сессиях) по имени поля
    /// </summary>
    public Task<ResponseBaseModel> ClearValuesForFieldName(FormFieldOfSessionModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить сессию опроса/анкеты
    /// </summary>
    public Task<ResponseBaseModel> DeleteSessionQuestionnaire(int session_id, CancellationToken cancellationToken = default);
    #endregion

    #region опросы/анкеты
    /// <summary>
    /// Обновить (или создать) анкету/опрос
    /// </summary>
    public Task<FormQuestionnaireResponseModel> UpdateOrCreateQuestionnaire(EntryDescriptionModel questionnaire, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запрос анкет/опросов
    /// </summary>
    public Task<ConstructorFormsQuestionnairesPaginationResponseModel> RequestQuestionnaires(SimplePaginationRequestModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить анкету/опрос
    /// </summary>
    public Task<FormQuestionnaireResponseModel> GetQuestionnaire(int questionnaire_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить опрос/анкету
    /// </summary>
    public Task<ResponseBaseModel> DeleteQuestionnaire(int questionnaire_id, CancellationToken cancellationToken = default);
    #endregion

    #region страницы опросов/анкет
    /// <summary>
    /// Обновить (или создать) страницу опроса/анкеты
    /// </summary>
    public Task<FormQuestionnairePageResponseModel> CreateOrUpdateQuestionnairePage(EntryDescriptionOwnedModel questionnaire_page, CancellationToken cancellationToken = default);

    /// <summary>
    /// Перемещение страницы опроса/анкеты (сортировка страниц внутри опроса/анкеты)
    /// </summary>
    public Task<FormQuestionnaireResponseModel> QuestionnairePageMove(int questionnaire_page_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить страницу анкеты/опроса
    /// </summary>
    public Task<FormQuestionnairePageResponseModel> GetQuestionnairePage(int questionnaire_page_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить страницу опроса/анкеты
    /// </summary>
    public Task<ResponseBaseModel> DeleteQuestionnairePage(int questionnaire_page_id, CancellationToken cancellationToken = default);
    #endregion

    #region структура страниц опросов/анкет: формы и настройки
    /// <summary>
    /// Обновить (или создать) связь [страницы анкеты/опроса] с [формой]
    /// </summary>
    public Task<ResponseBaseModel> CreateOrUpdateQuestionnairePageJoinForm(ConstructorFormQuestionnairePageJoinFormModelDB page_join_form, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть связь [формы] со [страницей анкеты/опроса] (изменение сортировки/последовательности)
    /// </summary>
    public Task<FormQuestionnairePageResponseModel> QuestionnairePageJoinFormMove(int questionnaire_page_join_form_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить данные по связи [формы] со [страницей анкеты/опроса]
    /// </summary>
    public Task<FormQuestionnairePageJoinFormResponseModel> GetQuestionnairePageJoinForm(int questionnaire_page_join_form_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить связь [формы] со [страницей анкеты/опроса]
    /// </summary>
    public Task<ResponseBaseModel> DeleteQuestionnairePageJoinForm(int questionnaire_page_join_form_id, CancellationToken cancellationToken = default);
    #endregion

    #region формы
    /// <summary>
    /// Подобрать формы
    /// </summary>
    public Task<ConstructorFormsPaginationResponseModel> SelectForms(AltSimplePaginationRequestModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить форму
    /// </summary>
    public Task<FormResponseModel> GetForm(int form_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать форму (имя, описание, `признак таблицы`)
    /// </summary>
    public Task<FormResponseModel> FormUpdateOrCreate(ConstructorFormBaseModel form, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить форму
    /// </summary>
    public Task<ResponseBaseModel> FormDelete(int form_id, CancellationToken cancellationToken = default);
    #endregion

    #region поля форм    
    /// <summary>
    /// Сдвинуть поле формы (простой тип)
    /// </summary>
    public Task<FormResponseModel> FieldFormMove(int field_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть поле формы (тип: список/справочник)
    /// </summary>
    public Task<FormResponseModel> FieldDirectoryFormMove(int field_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить сортировку и нормализовать в случае рассинхрона
    /// </summary>
    public Task<FormResponseModel> CheckAndNormalizeSortIndex(ConstructorFormModelDB form, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать поле формы (простой тип)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldUpdateOrCreate(ConstructorFieldFormBaseModel form_field, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить поле формы (простой тип)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldDelete(int form_field_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать поле формы (тип: справочник/список)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldDirectoryUpdateOrCreate(ConstructorFormDirectoryLinkModelDB field_directory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить поле формы (тип: справочник/список)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldDirectoryDelete(int field_directory_id, CancellationToken cancellationToken = default);
    #endregion

    #region справочники
    /// <summary>
    /// Прочитать данные справочников
    /// </summary>
    public Task<IEnumerable<EntryNestedModel>> ReadDirectories(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить справочники/списки
    /// </summary>
    public Task<EntriesResponseModel> GetDirectories(string? name_filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/Создать справочник
    /// </summary>
    public Task<CreateObjectOfIntKeyResponseModel> UpdateOrCreateDirectory(EntryModel _dir, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить справочник/список (со всеми элементами и связями)
    /// </summary>
    public Task<ResponseBaseModel> DeleteDirectory(int directory_id, CancellationToken cancellationToken = default);
    #endregion

    #region элементы справочников
    /// <summary>
    /// Получить элементы справочника/списка
    /// </summary>
    public Task<EntriesResponseModel> GetElementsOfDirectory(int directory_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать элемент справочника
    /// </summary>
    /// <param name="name_element_of_dir">Имя элемента (создаваемого)</param>
    /// <param name="directory_id">Справочник в который будет добавлен элемент</param>
    /// <param name="cancellationToken"></param>
    public Task<CreateObjectOfIntKeyResponseModel> CreateElementForDirectory(string name_element_of_dir, int directory_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить элемент справочника
    /// </summary>
    public Task<ResponseBaseModel> UpdateElementOfDirectory(EntryModel element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить элемент справочника/списка
    /// </summary>
    public Task<ResponseBaseModel> DeleteElementFromDirectory(int element_id, CancellationToken cancellationToken = default);
    #endregion
}