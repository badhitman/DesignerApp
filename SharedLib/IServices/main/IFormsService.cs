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
    public Task<TResponseModel<ConstructorFormSessionModelDB>> GetSessionQuestionnaire(string guid_session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Установить значение свойства сессии
    /// </summary>
    public Task<TResponseModel<ConstructorFormSessionModelDB>> SetValueFieldSessionQuestionnaire(SetValueFieldSessionQuestionnaireModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить опрос на проверку (от клиента)
    /// </summary>
    public Task<ResponseBaseModel> SetDoneSessionQuestionnaire(string token_session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить набор значений сессии опроса/анкеты по номеру строки [GroupByRowNum].
    /// Если индекс ниже нуля - удаляются все значения для указанной JoinForm (полная очистка таблицы или очистка всех значений всех поллей стандартной формы)
    /// </summary>
    public Task<ResponseBaseModel> DeleteValuesFieldsByGroupSessionQuestionnaireByRowNum(ValueFieldSessionQuestionnaireBaseModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить новую строку в таблицу значений
    /// </summary>
    /// <returns>Номер п/п (начиная с 1) созданной строки</returns>
    public Task<TResponseStrictModel<int>> AddRowToTable(FieldSessionQuestionnaireBaseModel req, CancellationToken cancellationToken = default);
    #endregion

    #region сессии опросов/анкет
    /// <summary>
    /// Установить статус сессии (от менеджера)
    /// </summary>
    public Task<ResponseBaseModel> SetStatusSessionQuestionnaire(int id_session, SessionsStatusesEnum status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить сессию
    /// </summary>
    public Task<TResponseModel<ConstructorFormSessionModelDB>> GetSessionQuestionnaire(int id_session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить (или создать) сессию опроса/анкеты
    /// </summary>
    public Task<TResponseModel<ConstructorFormSessionModelDB>> UpdateOrCreateSessionQuestionnaire(ConstructorFormSessionModelDB session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запросить порцию сессий (с пагинацией)
    /// </summary>
    public Task<ConstructorFormsSessionsPaginationResponseModel> RequestSessionsQuestionnaires(RequestSessionsQuestionnairesRequestPaginationModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Найти порцию сессий по имени поля (с пагинацией)
    /// </summary>
    public Task<TResponseModel<EntryDictModel[]>> FindSessionsQuestionnairesByFormFieldName(FormFieldModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить значения (введённые в сессиях) по имени поля
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
    public Task<TResponseModel<ConstructorFormQuestionnaireModelDB>> UpdateOrCreateQuestionnaire(EntryConstructedModel questionnaire, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запрос анкет/опросов
    /// </summary>
    public Task<ConstructorFormsQuestionnairesPaginationResponseModel> RequestQuestionnaires(SimplePaginationRequestModel req, int projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить анкету/опрос
    /// </summary>
    public Task<TResponseModel<ConstructorFormQuestionnaireModelDB>> GetQuestionnaire(int questionnaire_id, CancellationToken cancellationToken = default);

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
    public Task<TResponseModel<ConstructorFormQuestionnaireModelDB>> QuestionnairePageMove(int questionnaire_page_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

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
    public Task<TResponseModel<ConstructorFormQuestionnairePageJoinFormModelDB>> GetQuestionnairePageJoinForm(int questionnaire_page_join_form_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить связь [формы] со [страницей анкеты/опроса]
    /// </summary>
    public Task<ResponseBaseModel> DeleteQuestionnairePageJoinForm(int questionnaire_page_join_form_id, CancellationToken cancellationToken = default);
    #endregion

    #region формы
    /// <summary>
    /// Подобрать формы
    /// </summary>
    public Task<ConstructorFormsPaginationResponseModel> SelectForms(AltSimplePaginationRequestModel req, int projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить форму
    /// </summary>
    public Task<TResponseModel<ConstructorFormModelDB>> GetForm(int form_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать форму (имя, описание, `признак таблицы`)
    /// </summary>
    public Task<TResponseModel<ConstructorFormModelDB>> FormUpdateOrCreate(ConstructorFormBaseModel form, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить форму
    /// </summary>
    public Task<ResponseBaseModel> FormDelete(int form_id, CancellationToken cancellationToken = default);
    #endregion

    #region поля форм    
    /// <summary>
    /// Сдвинуть поле формы (простой тип)
    /// </summary>
    public Task<TResponseModel<ConstructorFormModelDB>> FieldFormMove(int field_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть поле формы (тип: список/справочник)
    /// </summary>
    public Task<TResponseModel<ConstructorFormModelDB>> FieldDirectoryFormMove(int field_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить сортировку и нормализовать в случае рассинхрона
    /// </summary>
    public Task<TResponseModel<ConstructorFormModelDB>> CheckAndNormalizeSortIndexFrmFields(ConstructorFormModelDB form, CancellationToken cancellationToken = default);

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
    /// Получить справочники/списки для проекта
    /// </summary>
    public Task<TResponseStrictModel<SystemEntryModel[]>> GetDirectories(int project_id, string? name_filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/Создать справочник
    /// </summary>
    public Task<TResponseStrictModel<int>> UpdateOrCreateDirectory(EntryConstructedModel _dir, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить справочник/список (со всеми элементами и связями)
    /// </summary>
    public Task<ResponseBaseModel> DeleteDirectory(int directory_id, CancellationToken cancellationToken = default);
    #endregion

    #region элементы справочников
    /// <summary>
    /// Получить элементы справочника/списка
    /// </summary>
    public Task<TResponseModel<SystemEntryModel[]>> GetElementsOfDirectory(int directory_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать элемент справочника
    /// </summary>
    public Task<TResponseStrictModel<int>> CreateElementForDirectory(SystemOwnedNameModel element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить элемент справочника
    /// </summary>
    public Task<ResponseBaseModel> UpdateElementOfDirectory(SystemEntryModel element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить элемент справочника/списка
    /// </summary>
    public Task<ResponseBaseModel> DeleteElementFromDirectory(int element_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть выше элемент справочника/списка
    /// </summary>
    public Task<ResponseBaseModel> UpMoveElementOfDirectory(int element_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть ниже элемент справочника/списка
    /// </summary>
    public Task<ResponseBaseModel> DownMoveElementOfDirectory(int element_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Нормализовать индексы сортировки элементов справочника.
    /// </summary>
    public Task<ResponseBaseModel> CheckAndNormalizeSortIndexForElementsOfDirectory(int directory_id, CancellationToken cancellationToken = default);
    #endregion

    #region проекты
    /// <summary>
    /// Получить проекты
    /// </summary>
    /// <param name="name_filter">фильтр по имени</param>
    /// <param name="for_user_id">чъи проекты (по умолчанию null - значит свои проекты)</param>
    public Task<ProjectViewModel[]> GetProjects(string for_user_id, string? name_filter = null);

    /// <summary>
    /// Прочитать данные проекта
    /// </summary>
    public Task<ProjectConstructorModelDb?> ReadProject(int project_id);

    /// <summary>
    /// Создать проект
    /// </summary>
    public Task<TResponseModel<int>> CreateProject(ProjectViewModel project, string owner_user_id);

    /// <summary>
    /// Установить проекту признак <c>IsDeleted</c>.
    /// </summary>
    public Task<ResponseBaseModel> SetMarkerDeleteProject(int project_id, bool is_deleted);

    /// <summary>
    /// Обновить проект
    /// </summary>
    public Task<ResponseBaseModel> UpdateProject(ProjectViewModel project);

    /// <summary>
    /// Добавить участника к проекту
    /// </summary>
    public Task<ResponseBaseModel> AddMemberToProject(int project_id, string member_user_id);

    /// <summary>
    /// Исключить участника из проекта
    /// </summary>
    public Task<ResponseBaseModel> DeleteMemberFromProject(int project_id, string member_user_id);

    /// <summary>
    /// Получить участников проекта (за исключением владельца, который хранится в самом проекте)
    /// </summary>
    public Task<EntryAltModel[]> GetMembersOfProject(int project_id);

    /// <summary>
    /// Установить проект как основной/используемый для пользователя.
    /// </summary>
    /// <param name="project_id">проект</param>
    /// <param name="user_id">если null, то для текущего пользователя</param>
    public Task<ResponseBaseModel> SetProjectAsMain(int project_id, string user_id);

    /// <summary>
    /// Получить текущий основной/используемый проект. Если <paramref name="user_id"/> == null, то для текущего пользователя
    /// </summary>
    public Task<TResponseModel<MainProjectViewModel>> GetCurrentMainProject(string user_id);
    #endregion
}