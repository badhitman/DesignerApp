////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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
    public Task<TResponseModel<SessionOfDocumentDataModelDB>> GetSessionQuestionnaire(string guid_session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Установить значение свойства сессии
    /// </summary>
    public Task<TResponseModel<SessionOfDocumentDataModelDB>> SetValueFieldSessionQuestionnaire(SetValueFieldSessionQuestionnaireModel req, CancellationToken cancellationToken = default);

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

    /////////////// Контекст работы конструктора: работы в системе над какими-либо сущностями всегда принадлежат какому-либо проекту/контексту.
    // При переключении контекста (текущий/основной проект) становятся доступны только работы по этому проекту
    // В проект можно добавлять участников, что бы те могли работать вместе с владельцем => вносить изменения в конструкторе данного проекта/контекста
    // Если проект отключить (есть у него такой статус: IsDisabled), то работы с проектом блокируются для всех участников, кроме владельца
    #region проекты
    /// <summary>
    /// Получить проекты
    /// </summary>
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
    /// Установить проекту признак <paramref name="is_deleted"/> <c>IsDeleted</c> .
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
    public Task<ResponseBaseModel> SetProjectAsMain(int project_id, string user_id);

    /// <summary>
    /// Получить текущий основной/используемый проект
    /// </summary>
    public Task<TResponseModel<MainProjectViewModel>> GetCurrentMainProject(string user_id);
    #endregion

    /////////////// Перечисления.
    // Простейший тип данных поля формы, который можно в в последствии использовать в конструкторе форм при добавлении/редактировании полей
    // Примечание: В генераторе для C# .NET формируются как Enum
    #region справочники/списки
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
    public Task<TResponseStrictModel<int>> UpdateOrCreateDirectory(EntryConstructedModel _dir, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить справочник/список (со всеми элементами и связями)
    /// </summary>
    public Task<ResponseBaseModel> DeleteDirectory(int directory_id, string call_as_user_id, CancellationToken cancellationToken = default);
    #endregion
    #region элементы справочникв/списков
    /// <summary>
    /// Получить элементы справочника/списка
    /// </summary>
    public Task<TResponseModel<List<SystemEntryModel>>> GetElementsOfDirectory(int directory_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать элемент справочника
    /// </summary>
    public Task<TResponseStrictModel<int>> CreateElementForDirectory(SystemOwnedNameModel element, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить элемент справочника
    /// </summary>
    public Task<ResponseBaseModel> UpdateElementOfDirectory(SystemEntryModel element, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить элемент справочника/списка
    /// </summary>
    public Task<ResponseBaseModel> DeleteElementFromDirectory(int element_id, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть выше элемент справочника/списка
    /// </summary>
    public Task<ResponseBaseModel> UpMoveElementOfDirectory(int element_id, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть ниже элемент справочника/списка
    /// </summary>
    public Task<ResponseBaseModel> DownMoveElementOfDirectory(int element_id, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Нормализовать индексы сортировки элементов справочника.
    /// </summary>
    public Task<ResponseBaseModel> CheckAndNormalizeSortIndexForElementsOfDirectory(int directory_id, CancellationToken cancellationToken = default);
    #endregion

    /////////////// Формы для редактирования/добавления бизнес-сущностей внутри итогового документа.
    // Базовая бизнес-сущность описывающая каркас/строение данных. Можно сравнить с таблицей БД со своим набором полей/колонок
    // К тому же сразу настраивается web-форма для редактирования объекта данного типа. Возможность устанавливать css стили формам и полям (с умыслом использования возможностей Bootstrap)
    // Тип данных для полей форм может быть любой из перечня доступных: перечисление (созданное вами же), строка, число, булево, дата и т.д.
    #region формы
    /// <summary>
    /// Подобрать формы
    /// </summary>
    public Task<ConstructorFormsPaginationResponseModel> SelectForms(SimplePaginationRequestModel req, int projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить форму
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> GetForm(int form_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать форму (имя, описание, `признак таблицы`)
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> FormUpdateOrCreate(ConstructorFormBaseModel form, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить форму
    /// </summary>
    public Task<ResponseBaseModel> FormDelete(int form_id, string call_as_user_id, CancellationToken cancellationToken = default);
    #endregion
    #region поля форм    
    /// <summary>
    /// Сдвинуть поле формы (простой тип)
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> FieldFormMove(int field_id, string call_as_user_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть поле формы (тип: список/справочник)
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> FieldDirectoryFormMove(int field_id, string call_as_user_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить сортировку и нормализовать в случае рассинхрона
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> CheckAndNormalizeSortIndexFrmFields(FormConstructorModelDB form, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать поле формы (простой тип)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldUpdateOrCreate(ConstructorFieldFormBaseModel form_field, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить поле формы (простой тип)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldDelete(int form_field_id, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать поле формы (тип: справочник/список)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldDirectoryUpdateOrCreate(LinkDirectoryToFormConstructorModelDB field_directory, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить поле формы (тип: справочник/список)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldDirectoryDelete(int field_directory_id, string call_as_user_id, CancellationToken cancellationToken = default);
    #endregion

    /////////////// Документ. Описывается/настраивается конечный результат, который будет использоваться.
    // Может содержать одну или несколько вкладок/табов. На каждом табе/вкладке может располагаться одна или больше форм
    // Каждая располагаемая форма может быть помечена как [Табличная]. Т.е. пользователь будет добавлять сколь угодно строк одной и той же формы.
    // Пользователь при добавлении/редактировании строк таблицы будет видеть форму, которую вы настроили для этого, а внутри таба это будет выглядеть как обычная многострочная таблица с колонками, равными полям формы
    #region документы
    /// <summary>
    /// Обновить (или создать) анкету/опрос
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB>> UpdateOrCreateQuestionnaire(EntryConstructedModel questionnaire, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запрос анкет/опросов
    /// </summary>
    public Task<ConstructorFormsQuestionnairesPaginationResponseModel> RequestQuestionnaires(SimplePaginationRequestModel req, int projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить анкету/опрос
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB>> GetQuestionnaire(int questionnaire_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить опрос/анкету
    /// </summary>
    public Task<ResponseBaseModel> DeleteQuestionnaire(int questionnaire_id, string call_as_user_id, CancellationToken cancellationToken = default);
    #endregion
    // табы/вкладки схожи по смыслу табов/вкладок в Excel. Т.е. обычная группировка разных рабочих пространств со своим именем 
    #region табы документов
    /// <summary>
    /// Обновить (или создать) страницу опроса/анкеты
    /// </summary>
    public Task<FormQuestionnairePageResponseModel> CreateOrUpdateQuestionnairePage(EntryDescriptionOwnedModel questionnaire_page, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Перемещение страницы опроса/анкеты (сортировка страниц внутри опроса/анкеты)
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB>> QuestionnairePageMove(int questionnaire_page_id, string call_as_user_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить страницу анкеты/опроса
    /// </summary>
    public Task<FormQuestionnairePageResponseModel> GetQuestionnairePage(int questionnaire_page_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить страницу опроса/анкеты
    /// </summary>
    public Task<ResponseBaseModel> DeleteQuestionnairePage(int questionnaire_page_id, string call_as_user_id, CancellationToken cancellationToken = default);
    #endregion
    #region структура/схема таба/вкладки: формы, порядок и настройки поведения
    /// <summary>
    /// Обновить (или создать) связь [страницы анкеты/опроса] с [формой]
    /// </summary>
    public Task<ResponseBaseModel> CreateOrUpdateQuestionnairePageJoinForm(TabJoinDocumentSchemeConstructorModelDB page_join_form, string call_as_user_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть связь [формы] со [страницей анкеты/опроса] (изменение сортировки/последовательности)
    /// </summary>
    public Task<FormQuestionnairePageResponseModel> QuestionnairePageJoinFormMove(int questionnaire_page_join_form_id, string call_as_user_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить данные по связи [формы] со [страницей анкеты/опроса]
    /// </summary>
    public Task<TResponseModel<TabJoinDocumentSchemeConstructorModelDB>> GetQuestionnairePageJoinForm(int questionnaire_page_join_form_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить связь [формы] со [страницей анкеты/опроса]
    /// </summary>
    public Task<ResponseBaseModel> DeleteQuestionnairePageJoinForm(int questionnaire_page_join_form_id, string call_as_user_id, CancellationToken cancellationToken = default);
    #endregion

    /////////////// Пользовательский/публичный доступ к возможностям заполнения документа данными
    // Если у вас есть готовый к заполнению документ со всеми его табами и настройками, то вы можете создавать уникальные ссылки для заполнения данными
    // Каждая ссылка это всего лишь уникальный GUID к которому привязываются все данные, которые вводят конечные пользователи
    // Пользователи видят ваш документ, но сам документ данные не хранит. Хранение данных происходит в сессиях, которые вы сами выпускаете для любого вашего документа
    #region сессии опросов/анкет
    /// <summary>
    /// Установить статус сессии (от менеджера)
    /// </summary>
    public Task<ResponseBaseModel> SetStatusSessionQuestionnaire(int id_session, SessionsStatusesEnum status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить сессию
    /// </summary>
    public Task<TResponseModel<SessionOfDocumentDataModelDB>> GetSessionQuestionnaire(int id_session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить (или создать) сессию опроса/анкеты
    /// </summary>
    public Task<TResponseModel<SessionOfDocumentDataModelDB>> UpdateOrCreateSessionQuestionnaire(SessionOfDocumentDataModelDB session, CancellationToken cancellationToken = default);

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
}