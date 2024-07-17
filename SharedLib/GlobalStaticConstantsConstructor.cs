namespace SharedLib;

public static partial class GlobalStaticConstants
{
    /// <summary>
    /// Префикс модели rest/ответа одного элемента
    /// </summary>
    public const string SINGLE_REPONSE_MODEL_PREFIX = "ResponseModel";

    /// <summary>
    /// Префикс модели rest/ответа коллекции элементов
    /// </summary>
    public const string MULTI_REPONSE_MODEL_PREFIX = "ResponseListModel";

    /// <summary>
    /// Префикс модели rest/ответа с поддержкой пагинации
    /// </summary>
    public const string PAGINATION_REPONSE_MODEL_PREFIX = "ResponsePaginationModel";

    /// <summary>
    /// Имя поля результат полезной нагрузки
    /// </summary>
    public const string RESULT_PROPERTY_NAME = "Result";

    /// <summary>
    /// Префикс имени поля (имя таблицы БД) для контекста базы данных
    /// </summary>
    public const string CONTEXT_DATA_SET_PREFIX = "DbSet";

    /// <summary>
    /// Префикс сервиса непосредственного доступа к данным БД
    /// </summary>
    public const string DATABASE_TABLE_ACESSOR_PREFIX = "TableAccessor";

    /// <summary>
    /// Префикс сервиса/прокладки между контроллером и сервисом доступа к данным
    /// </summary>
    public const string SERVICE_ACESSOR_PREFIX = "Service";

    /// <summary>
    /// Префикс имени поля данных для документа-объекта
    /// </summary>
    public const string TABLE_PROPERTY_NAME_PREFIX = "TableProperty";
}