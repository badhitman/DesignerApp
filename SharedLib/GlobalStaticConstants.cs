////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Константы
/// </summary>
public static class GlobalStaticConstants
{
    /// <summary>
    /// Identity.StatusMessage
    /// </summary>
    public const string StatusCookieName = "Identity.StatusMessage";

    /// <summary>
    /// Identity LoginCallback action name
    /// </summary>
    public const string LoginCallbackAction = "LoginCallback";

    #region имена полей для хранения информации о сессии

    /// <summary>
    /// имя поля в storage браузера для хранения информации о 'id пользователя' сессии
    /// </summary>
    public const string SESSION_STORAGE_KEY_USER_ID = "session_user_id";

    /// <summary>
    /// имя поля в storage браузера для хранения информации о 'логине' сессии
    /// </summary>
    public const string SESSION_STORAGE_KEY_LOGIN = "session_login";

    /// <summary>
    /// имя поля в storage браузера для хранения информации о 'уровне доступа' сессии
    /// </summary>
    public const string SESSION_STORAGE_KEY_LEVEL = "session_level";

    /// <summary>
    /// имя поля в storage браузера для хранения информации о 'токене' сессии
    /// </summary>
    public const string SESSION_STORAGE_KEY_TOKEN = "session_token";

    #endregion

    /// <summary>
    /// Имя заголовка для передачи токена от клиента к серверу
    /// </summary>
    public const string SESSION_TOKEN_NAME = "token";

    /// <summary>
    /// имена контроллеров и действий
    /// </summary>
    public static class Routes
    {
        /// <summary>
        /// Аутентификация
        /// </summary>
        public const string AUTHENTICATION_CONTROLLER_NAME = "authentication";

        /// <summary>
        /// Пользователи
        /// </summary>
        public const string USERS_CONTROLLER_NAME = "users";

        /// <summary>
        /// Перечисления (enum`s)
        /// </summary>
        public const string ENUMS_CONTROLLER_NAME = "enums";

        /// <summary>
        /// Документы
        /// </summary>
        public const string DOCUMENTS_CONTROLLER_NAME = "documents";

        /// <summary>
        /// Проекты
        /// </summary>
        public const string PROJECTS_CONTROLLER_NAME = "projects";

        /// <summary>
        /// Telegram check-user
        /// </summary>
        public const string TELEGRAM_CONTROLLER_NAME = "telegram";




        /// <summary>
        /// Редактировать
        /// </summary>
        public const string EDIT_ACTION_NAME = "edit";

        /// <summary>
        /// Check
        /// </summary>
        public const string CHECK_ACTION_NAME = "check";

        /// <summary>
        /// Список
        /// </summary>
        public const string LIST_ACTION_NAME = "list";

        /// <summary>
        /// Профиль
        /// </summary>
        public const string PROFILE_ACTION_NAME = "profile";

        /// <summary>
        /// Выйти
        /// </summary>
        public const string LOGOUT_ACTION_NAME = "logout";

        /// <summary>
        /// Войти
        /// </summary>
        public const string LOGIN_ACTION_NAME = "login";

        /// <summary>
        /// Восстановить
        /// </summary>
        public const string RESTORE_ACTION_NAME = "restore";

        /// <summary>
        /// Регистрация
        /// </summary>
        public const string REGISTRATION_ACTION_NAME = "registration";
    }

    /// <summary>
    /// Имя пространства имён мемкеша для хранения сессий
    /// </summary>
    public const string SESSION_MEMCASHE_NAMESPASE = "sessions";

    /// <summary>
    /// Шаблон системного кодового имени
    /// </summary>
    public const string SYSTEM_CODE_NAME_TEMPLATE = @"^[a-zA-Z][a-zA-Z0-9_]{1,72}[a-zA-Z0-9]{1,72}$";

    /// <summary>
    /// Шаблон пространства имён
    /// </summary>
    public const string NAME_SPACE_TEMPLATE = @"^[a-zA-Z][a-zA-Z0-9_\.]{1,72}[a-zA-Z0-9]{1,128}$";

    /// <summary>
    /// Имя очереди Telegram
    /// </summary>
    public const string QueueResponseTelegramHeaderName = "QueueTelegramHeader";
}