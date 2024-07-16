////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Constructor служба
/// </summary>
public interface IConstructorService
{
    #region public
    /// <summary>
    /// Получить сессию
    /// </summary>
    public Task<TResponseModel<SessionOfDocumentDataModelDB>> GetSessionDocumentData(string guid_session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Установить значение свойства сессии
    /// </summary>
    public Task<TResponseModel<SessionOfDocumentDataModelDB>> SetValueFieldSessionDocumentData(SetValueFieldDocumentDataModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить опрос на проверку (от клиента)
    /// </summary>
    public Task<ResponseBaseModel> SetDoneSessionDocumentData(string token_session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить набор значений сессии опроса/анкеты по номеру строки [GroupByRowNum].
    /// Если индекс ниже нуля - удаляются все значения для указанной JoinForm (полная очистка таблицы или очистка всех значений всех поллей стандартной формы)
    /// </summary>
    public Task<ResponseBaseModel> DeleteValuesFieldsByGroupSessionDocumentDataByRowNum(ValueFieldSessionDocumentDataBaseModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить новую строку в таблицу значений
    /// </summary>
    /// <returns>Номер п/п (начиная с 1) созданной строки</returns>
    public Task<TResponseStrictModel<int>> AddRowToTable(FieldSessionDocumentDataBaseModel req, CancellationToken cancellationToken = default);
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
    public Task<ProjectConstructorModelDB?> ReadProject(int project_id);

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

    /// <summary>
    /// Проверка пользователя на возможность проводить работы в контексте проекта.
    /// </summary>
    /// <remarks>
    /// Пользователи с ролью ADMIN имеют полный доступ ко всем проектам.
    /// Владельцы имеют полный доступ к своим проектам, а простые участники проектов зависят от статуса проекта (выкл/вкл)
    /// </remarks>
    public Task<ResponseBaseModel> CanEditProject(int project_id, string? user_id = null);

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
    public Task<TResponseStrictModel<EntryModel[]>> GetDirectories(int project_id, string? name_filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить справочник (список/перечисление)
    /// </summary>
    public Task<EntryDescriptionModel> GetDirectory(int enumeration_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/Создать справочник
    /// </summary>
    public Task<TResponseStrictModel<int>> UpdateOrCreateDirectory(EntryConstructedModel _dir, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить справочник/список (со всеми элементами и связями)
    /// </summary>
    public Task<ResponseBaseModel> DeleteDirectory(int directory_id, CancellationToken cancellationToken = default);
    #endregion
    #region элементы справочникв/списков
    /// <summary>
    /// Получить элементы справочника/списка
    /// </summary>
    public Task<TResponseModel<List<EntryModel>>> GetElementsOfDirectory(int directory_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать элемент справочника
    /// </summary>
    public Task<TResponseStrictModel<int>> CreateElementForDirectory(OwnedNameModel element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить элемент справочника
    /// </summary>
    public Task<ResponseBaseModel> UpdateElementOfDirectory(EntryDescriptionModel element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить элемент справочника/перечисления/списка
    /// </summary>
    public Task<EntryDescriptionModel> GetElementOfDirectory(int element_id, CancellationToken cancellationToken = default);

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
    public Task<TResponseModel<FormConstructorModelDB>> FormUpdateOrCreate(FormBaseConstructorModel form, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить форму
    /// </summary>
    public Task<ResponseBaseModel> FormDelete(int form_id, CancellationToken cancellationToken = default);
    #endregion
    #region поля форм    
    /// <summary>
    /// Сдвинуть поле формы (простой тип)
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> FieldFormMove(int field_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть поле формы (тип: список/справочник)
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> FieldDirectoryFormMove(int field_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить сортировку и нормализовать в случае рассинхрона
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> CheckAndNormalizeSortIndexFrmFields(int form_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать поле формы (простой тип)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldUpdateOrCreate(FieldFormBaseConstructorModel form_field, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить поле формы (простой тип)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldDelete(int form_field_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать поле формы (тип: справочник/список)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldDirectoryUpdateOrCreate(FieldFormAkaDirectoryConstructorModelDB field_directory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить поле формы (тип: справочник/список)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldDirectoryDelete(int field_directory_id, CancellationToken cancellationToken = default);
    #endregion

    /////////////// Документ. Описывается/настраивается конечный результат, который будет использоваться.
    // Может содержать одну или несколько вкладок/табов. На каждом табе/вкладке может располагаться одна или больше форм
    // Каждая располагаемая форма может быть помечена как [Табличная]. Т.е. пользователь будет добавлять сколь угодно строк одной и той же формы.
    // Пользователь при добавлении/редактировании строк таблицы будет видеть форму, которую вы настроили для этого, а внутри таба это будет выглядеть как обычная многострочная таблица с колонками, равными полям формы
    #region документы
    /// <summary>
    /// Обновить/создать схему документа
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB>> UpdateOrCreateDocumentScheme(EntryConstructedModel documentScheme, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запрос схем документов
    /// </summary>
    public Task<ConstructorFormsDocumentSchemePaginationResponseModel> RequestDocumentsSchemes(SimplePaginationRequestModel req, int projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить схему документа
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB>> GetDocumentScheme(int questionnaire_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить схему документа
    /// </summary>
    public Task<ResponseBaseModel> DeleteDocumentScheme(int questionnaire_id, CancellationToken cancellationToken = default);
    #endregion
    // табы/вкладки схожи по смыслу табов/вкладок в Excel. Т.е. обычная группировка разных рабочих пространств со своим именем 
    #region табы документов
    /// <summary>
    /// Обновить/создать таб/вкладку схемы документа
    /// </summary>
    public Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> CreateOrUpdateTabOfDocumentScheme(EntryDescriptionOwnedModel questionnaire_page, CancellationToken cancellationToken = default);

    /// <summary>
    /// Перемещение страницы опроса/анкеты (сортировка страниц внутри опроса/анкеты)
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB>> MoveTabOfDocumentScheme(int questionnaire_page_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить страницу анкеты/опроса
    /// </summary>
    public Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> GetTabOfDocumentScheme(int questionnaire_page_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить страницу опроса/анкеты
    /// </summary>
    public Task<ResponseBaseModel> DeleteTabOfDocumentScheme(int questionnaire_page_id, CancellationToken cancellationToken = default);
    #endregion
    #region структура/схема таба/вкладки: формы, порядок и настройки поведения    
    /// <summary>
    /// Получить связь [таба/вкладки схемы документа] с [формой]
    /// </summary>
    public Task<TResponseModel<TabJoinDocumentSchemeConstructorModelDB>> GetTabDocumentSchemeJoinForm(int questionnaire_page_join_form_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать связь [таба/вкладки схемы документа] с [формой]
    /// </summary>
    public Task<ResponseBaseModel> CreateOrUpdateTabDocumentSchemeJoinForm(TabJoinDocumentSchemeConstructorModelDB page_join_form, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть связь [таба/вкладки схемы документа] с [формой] (изменение сортировки/последовательности)
    /// </summary>
    public Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> MoveTabDocumentSchemeJoinForm(int questionnaire_page_join_form_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить связь [таба/вкладки схемы документа] с [формой] 
    /// </summary>
    public Task<ResponseBaseModel> DeleteTabDocumentSchemeJoinForm(int questionnaire_page_join_form_id, CancellationToken cancellationToken = default);
    #endregion

    /////////////// Пользовательский/публичный доступ к возможностям заполнения документа данными
    // Если у вас есть готовый к заполнению документ со всеми его табами и настройками, то вы можете создавать уникальные ссылки для заполнения данными
    // Каждая ссылка это всего лишь уникальный GUID к которому привязываются все данные, которые вводят конечные пользователи
    // Пользователи видят ваш документ, но сам документ данные не хранит. Хранение данных происходит в сессиях, которые вы сами выпускаете для любого вашего документа
    #region сессии опросов/анкет
    /// <summary>
    /// Установить статус сессии (от менеджера)
    /// </summary>
    public Task<ResponseBaseModel> SetStatusSessionDocument(int id_session, SessionsStatusesEnum status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить сессию
    /// </summary>
    public Task<TResponseModel<SessionOfDocumentDataModelDB>> GetSessionDocument(int id_session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить (или создать) сессию опроса/анкеты
    /// </summary>
    public Task<TResponseModel<SessionOfDocumentDataModelDB>> UpdateOrCreateSessionDocument(SessionOfDocumentDataModelDB session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запросить порцию сессий (с пагинацией)
    /// </summary>
    public Task<ConstructorFormsSessionsPaginationResponseModel> RequestSessionsDocuments(RequestSessionsDocumentsRequestPaginationModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Найти порцию сессий по имени поля (с пагинацией)
    /// </summary>
    public Task<TResponseModel<EntryDictModel[]>> FindSessionsDocumentsByFormFieldName(FormFieldModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить значения (введённые в сессиях) по имени поля
    /// </summary>
    public Task<ResponseBaseModel> ClearValuesForFieldName(FormFieldOfSessionModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить сессию опроса/анкеты
    /// </summary>
    public Task<ResponseBaseModel> DeleteSessionDocument(int session_id, CancellationToken cancellationToken = default);
    #endregion
}