////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Constructor Remote Transmission Service
/// </summary>
public interface IConstructorRemoteTransmissionService
{
    #region public
    /// <summary>
    /// AddRowToTable
    /// </summary>
    public Task<TResponseModel<int>> AddRowToTable(FieldSessionDocumentDataBaseModel req);

    /// <summary>
    /// DeleteValuesFieldsByGroupSessionDocumentDataByRowNum
    /// </summary>
    public Task<ResponseBaseModel> DeleteValuesFieldsByGroupSessionDocumentDataByRowNum(ValueFieldSessionDocumentDataBaseModel req);

    /// <summary>
    /// SetDoneSessionDocumentData
    /// </summary>
    public Task<ResponseBaseModel> SetDoneSessionDocumentData(string req);

    /// <summary>
    /// SetValueFieldSessionDocumentData
    /// </summary>
    public Task<TResponseModel<SessionOfDocumentDataModelDB>> SetValueFieldSessionDocumentData(SetValueFieldDocumentDataModel req);

    /// <summary>
    /// GetSessionDocumentData
    /// </summary>
    public Task<TResponseModel<SessionOfDocumentDataModelDB>> GetSessionDocumentData(string req);
    #endregion

    #region derictories
    /// <summary>
    /// GetDirectory
    /// </summary>
    public Task<TResponseModel<EntryDescriptionModel>> GetDirectory(int req);

    /// <summary>
    /// GetDirectories
    /// </summary>
    public Task<TResponseModel<EntryModel[]>> GetDirectories(ProjectFindModel req);

    /// <summary>
    /// ReadDirectories
    /// </summary>
    public Task<List<EntryNestedModel>> ReadDirectories(int[] req);

    /// <summary>
    /// UpdateOrCreateDirectory
    /// </summary>
    public Task<TResponseModel<int>> UpdateOrCreateDirectory(TAuthRequestModel<EntryConstructedModel> req);

    /// <summary>
    /// DeleteDirectory
    /// </summary>
    public Task<ResponseBaseModel> DeleteDirectory(TAuthRequestModel<int> req);
    #endregion
    #region elements of directories
    /// <summary>
    /// GetElementsOfDirectory
    /// </summary>
    public Task<TResponseModel<List<EntryModel>>> GetElementsOfDirectory(int req);

    /// <summary>
    /// CreateElementForDirectory
    /// </summary>
    public Task<TResponseModel<int>> CreateElementForDirectory(TAuthRequestModel<OwnedNameModel> req);

    /// <summary>
    /// UpdateElementOfDirectory
    /// </summary>
    public Task<ResponseBaseModel> UpdateElementOfDirectory(TAuthRequestModel<EntryDescriptionModel> req);

    /// <summary>
    /// GetElementOfDirectory
    /// </summary>
    public Task<TResponseModel<EntryDescriptionModel>> GetElementOfDirectory(int req);

    /// <summary>
    /// DeleteElementFromDirectory
    /// </summary>
    public Task<ResponseBaseModel> DeleteElementFromDirectory(TAuthRequestModel<int> req);

    /// <summary>
    /// UpMoveElementOfDirectory
    /// </summary>
    public Task<ResponseBaseModel> UpMoveElementOfDirectory(TAuthRequestModel<int> req);

    /// <summary>
    /// DownMoveElementOfDirectory
    /// </summary>
    public Task<ResponseBaseModel> DownMoveElementOfDirectory(TAuthRequestModel<int> req);

    /// <summary>
    /// CheckAndNormalizeSortIndexForElementsOfDirectory
    /// </summary>
    public Task<ResponseBaseModel> CheckAndNormalizeSortIndexForElementsOfDirectory(int req);
    #endregion

    #region project
    /// <summary>
    /// CanEditProject
    /// </summary>
    public Task<ResponseBaseModel> CanEditProject(UserProjectModel req);

    /// <summary>
    /// DeleteMembersFromProject
    /// </summary>
    public Task<ResponseBaseModel> DeleteMembersFromProject(UsersProjectModel req);

    /// <summary>
    /// ProjectsRead
    /// </summary>
    public Task<List<ProjectModelDb>> ProjectsRead(int[] ids);

    /// <summary>
    /// GetProjectsForUser
    /// </summary>
    public Task<TResponseModel<ProjectViewModel[]>> GetProjectsForUser(GetProjectsForUserRequestModel req);

    /// <summary>
    /// SetMarkerDeleteProject
    /// </summary>
    public Task<ResponseBaseModel> SetMarkerDeleteProject(SetMarkerProjectRequestModel req);

    /// <summary>
    /// UpdateProject
    /// </summary>
    public Task<ResponseBaseModel> UpdateProject(ProjectViewModel req);

    /// <summary>
    /// AddMembersToProject
    /// </summary>
    public Task<ResponseBaseModel> AddMembersToProject(UsersProjectModel req);

    /// <summary>
    /// SetProjectAsMain
    /// </summary>
    public Task<ResponseBaseModel> SetProjectAsMain(UserProjectModel req);

    /// <summary>
    /// GetCurrentMainProject
    /// </summary>
    public Task<TResponseModel<MainProjectViewModel>> GetCurrentMainProject(string req);

    /// <summary>
    /// CreateProject
    /// </summary>
    public Task<TResponseModel<int>> CreateProject(CreateProjectRequestModel req);

    /// <summary>
    /// GetMembersOfProject
    /// </summary>
    public Task<TResponseModel<EntryAltModel[]>> GetMembersOfProject(int req);
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