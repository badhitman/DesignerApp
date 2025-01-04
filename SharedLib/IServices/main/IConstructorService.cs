////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
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
    public Task<ProjectViewModel[]> GetProjectsForUser(GetProjectsForUserRequestModel req);

    /// <summary>
    /// Прочитать данные проектов
    /// </summary>
    public Task<ProjectModelDb[]> ReadProjects(int[] projects_ids);

    /// <summary>
    /// Создать проект
    /// </summary>
    public Task<TResponseModel<int>> CreateProject(CreateProjectRequestModel req);

    /// <summary>
    /// Установить проекту признак <paramref name="req"/> <c>Marker</c> .
    /// </summary>
    public Task<ResponseBaseModel> SetMarkerDeleteProject(SetMarkerProjectRequestModel req);

    /// <summary>
    /// Обновить проект
    /// </summary>
    public Task<ResponseBaseModel> UpdateProject(ProjectViewModel project);

    /// <summary>
    /// Добавить участника к проекту
    /// </summary>
    public Task<ResponseBaseModel> AddMemberToProject(UsersProjectModel req);

    /// <summary>
    /// Исключить участника из проекта
    /// </summary>
    public Task<ResponseBaseModel> DeleteMembersFromProject(UsersProjectModel req);

    /// <summary>
    /// Получить участников проекта (за исключением владельца, который хранится в самом проекте)
    /// </summary>
    public Task<EntryAltModel[]> GetMembersOfProject(int project_id);

    /// <summary>
    /// Установить проект как основной/используемый для пользователя.
    /// </summary>
    public Task<ResponseBaseModel> SetProjectAsMain(UserProjectModel req);

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
    public Task<ResponseBaseModel> CanEditProject(UserProjectModel req);

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
    public Task<TResponseStrictModel<EntryModel[]>> GetDirectories(ProjectFindModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить справочник (список/перечисление)
    /// </summary>
    public Task<EntryDescriptionModel> GetDirectory(int enumeration_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/Создать справочник
    /// </summary>
    public Task<TResponseStrictModel<int>> UpdateOrCreateDirectory(TAuthRequestModel<EntryConstructedModel> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить справочник/список (со всеми элементами и связями)
    /// </summary>
    public Task<ResponseBaseModel> DeleteDirectory(TAuthRequestModel<int> req, CancellationToken cancellationToken = default);
    #endregion
    #region элементы справочникв/списков
    /// <summary>
    /// Получить элементы справочника/списка
    /// </summary>
    public Task<TResponseModel<List<EntryModel>>> GetElementsOfDirectory(int directory_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать элемент справочника
    /// </summary>
    public Task<TResponseStrictModel<int>> CreateElementForDirectory(TAuthRequestModel<OwnedNameModel> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить элемент справочника
    /// </summary>
    public Task<ResponseBaseModel> UpdateElementOfDirectory(TAuthRequestModel<EntryDescriptionModel> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить элемент справочника/перечисления/списка
    /// </summary>
    public Task<EntryDescriptionModel> GetElementOfDirectory(int element_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить элемент справочника/списка
    /// </summary>
    public Task<ResponseBaseModel> DeleteElementFromDirectory(TAuthRequestModel<int> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть выше элемент справочника/списка
    /// </summary>
    public Task<ResponseBaseModel> UpMoveElementOfDirectory(TAuthRequestModel<int> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть ниже элемент справочника/списка
    /// </summary>
    public Task<ResponseBaseModel> DownMoveElementOfDirectory(TAuthRequestModel<int> req, CancellationToken cancellationToken = default);

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
    public Task<TPaginationResponseModel<FormConstructorModelDB>> SelectForms(SelectFormsModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить форму
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> GetForm(int form_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать форму (имя, описание, `признак таблицы`)
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> FormUpdateOrCreate(TAuthRequestModel<FormBaseConstructorModel> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить форму
    /// </summary>
    public Task<ResponseBaseModel> FormDelete(TAuthRequestModel<int> req, CancellationToken cancellationToken = default);
    #endregion

    #region поля форм    
    /// <summary>
    /// Сдвинуть поле формы (простой тип)
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> FieldFormMove(TAuthRequestModel<MoveObjectModel> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть поле формы (тип: список/справочник)
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> FieldDirectoryFormMove(TAuthRequestModel<MoveObjectModel> req, CancellationToken cancellationToken = default);


    /// <summary>
    /// Проверить сортировку и нормализовать в случае рассинхрона
    /// </summary>
    public Task<TResponseModel<FormConstructorModelDB>> CheckAndNormalizeSortIndexFrmFields(int form_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать поле формы (простой тип)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldUpdateOrCreate(TAuthRequestModel<FieldFormConstructorModelDB> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить поле формы (простой тип)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldDelete(TAuthRequestModel<int> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать поле формы (тип: справочник/список)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldDirectoryUpdateOrCreate(TAuthRequestModel<FieldFormAkaDirectoryConstructorModelDB> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить поле формы (тип: справочник/список)
    /// </summary>
    public Task<ResponseBaseModel> FormFieldDirectoryDelete(TAuthRequestModel<int> req, CancellationToken cancellationToken = default);
    #endregion

    /////////////// Документ. Описывается/настраивается конечный результат, который будет использоваться.
    // Может содержать одну или несколько вкладок/табов. На каждом табе/вкладке может располагаться одна или больше форм
    // Каждая располагаемая форма может быть помечена как [Табличная]. Т.е. пользователь будет добавлять сколь угодно строк одной и той же формы.
    // Пользователь при добавлении/редактировании строк таблицы будет видеть форму, которую вы настроили для этого, а внутри таба это будет выглядеть как обычная многострочная таблица с колонками, равными полям формы
    #region документы
    /// <summary>
    /// Обновить/создать схему документа
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB>> UpdateOrCreateDocumentScheme(TAuthRequestModel<EntryConstructedModel> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запрос схем документов
    /// </summary>
    public Task<TPaginationResponseModel<DocumentSchemeConstructorModelDB>> RequestDocumentsSchemes(RequestDocumentsSchemesModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить схему документа
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB>> GetDocumentScheme(int questionnaire_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить схему документа
    /// </summary>
    public Task<ResponseBaseModel> DeleteDocumentScheme(TAuthRequestModel<int> req, CancellationToken cancellationToken = default);
    #endregion
    // табы/вкладки схожи по смыслу табов/вкладок в Excel. Т.е. обычная группировка разных рабочих пространств со своим именем 
    #region табы документов
    /// <summary>
    /// Обновить/создать таб/вкладку схемы документа
    /// </summary>
    public Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> CreateOrUpdateTabOfDocumentScheme(TAuthRequestModel<EntryDescriptionOwnedModel> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Перемещение страницы опроса/анкеты (сортировка страниц внутри опроса/анкеты)
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB>> MoveTabOfDocumentScheme(TAuthRequestModel<MoveObjectModel> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить страницу анкеты/опроса
    /// </summary>
    public Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> GetTabOfDocumentScheme(int questionnaire_page_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить страницу опроса/анкеты
    /// </summary>
    public Task<ResponseBaseModel> DeleteTabOfDocumentScheme(TAuthRequestModel<int> req, CancellationToken cancellationToken = default);
    #endregion
    #region структура/схема таба/вкладки: формы, порядок и настройки поведения    
    /// <summary>
    /// Получить связь [таба/вкладки схемы документа] с [формой]
    /// </summary>
    public Task<TResponseModel<FormToTabJoinConstructorModelDB>> GetTabDocumentSchemeJoinForm(int questionnaire_page_join_form_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить/создать связь [таба/вкладки схемы документа] с [формой]
    /// </summary>
    public Task<ResponseBaseModel> CreateOrUpdateTabDocumentSchemeJoinForm(TAuthRequestModel<FormToTabJoinConstructorModelDB> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сдвинуть связь [таба/вкладки схемы документа] с [формой] (изменение сортировки/последовательности)
    /// </summary>
    public Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> MoveTabDocumentSchemeJoinForm(TAuthRequestModel<MoveObjectModel> req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить связь [таба/вкладки схемы документа] с [формой] 
    /// </summary>
    public Task<ResponseBaseModel> DeleteTabDocumentSchemeJoinForm(TAuthRequestModel<int> req, CancellationToken cancellationToken = default);
    #endregion

    /////////////// Пользовательский/публичный доступ к возможностям заполнения документа данными
    // Если у вас есть готовый к заполнению документ со всеми его табами и настройками, то вы можете создавать уникальные ссылки для заполнения данными
    // Каждая ссылка это всего лишь уникальный GUID к которому привязываются все данные, которые вводят конечные пользователи
    // Пользователи видят ваш документ, но сам документ данные не хранит. Хранение данных происходит в сессиях, которые вы сами выпускаете для любого вашего документа
    #region сессии опросов/анкет
    /// <summary>
    /// Сохранить данные формы документа из сессии
    /// </summary>
    public Task<TResponseModel<ValueDataForSessionOfDocumentModelDB[]>> SaveSessionForm(SaveConstructorSessionRequestModel req);

    /// <summary>
    /// Установить статус сессии (от менеджера)
    /// </summary>
    public Task<ResponseBaseModel> SetStatusSessionDocument(SessionStatusModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить сессию
    /// </summary>
    public Task<TResponseModel<SessionOfDocumentDataModelDB>> GetSessionDocument(SessionGetModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить (или создать) сессию опроса/анкеты
    /// </summary>
    public Task<TResponseModel<SessionOfDocumentDataModelDB>> UpdateOrCreateSessionDocument(SessionOfDocumentDataModelDB session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запросить порцию сессий (с пагинацией)
    /// </summary>
    public Task<TPaginationResponseModel<SessionOfDocumentDataModelDB>> RequestSessionsDocuments(RequestSessionsDocumentsRequestPaginationModel req, CancellationToken cancellationToken = default);

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