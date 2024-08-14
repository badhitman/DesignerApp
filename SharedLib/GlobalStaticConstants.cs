////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Константы
/// </summary>
public static partial class GlobalStaticConstants
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
    /// Префикс имени MQ очереди
    /// </summary>
    public const string TransmissionQueueNamePrefix = "Transmission.Receives";

    /// <summary>
    /// Cloud storage metadata
    /// </summary>
    public static class CloudStorageMetadata
    {
        /// <inheritdoc/>
        public static StorageCloudParameterModel ParameterShowDisabledRubrics => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = Path.Combine(Routes.RUBRIC_CONTROLLER_NAME, Routes.FORM_CONTROLLER_NAME, $"{Routes.SHOW_ACTION_NAME}-{Routes.DISABLED_CONTROLLER_NAME}"),
        };

        /// <inheritdoc/>
        public static StorageCloudParameterModel ModeSelectingRubrics => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = Path.Combine(Routes.RUBRIC_CONTROLLER_NAME, Routes.FORM_CONTROLLER_NAME, $"{Routes.MODE_ACTION_NAME}-{Routes.SELECT_ACTION_NAME}"),
        };
    }

    /// <summary>
    /// Transmission MQ queues
    /// </summary>
    public static class TransmissionQueues
    {
        #region Web
        /// <inheritdoc/>
        public readonly static string FindUsersOfIdentityReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.IDENTITY_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, Routes.FIND_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetTelegramUserReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.USERS_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetWebConfigReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.WEB_CONTROLLER_NAME, Routes.CONFIGURATION_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string TelegramJoinAccountConfirmReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.JOIN_ACTION_NAME, Routes.ACCOUNT_CONTROLLER_NAME, Routes.CONFIRM_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string TelegramJoinAccountDeleteReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.JOIN_ACTION_NAME, Routes.ACCOUNT_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string UpdateTelegramMainUserMessageReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.MAIN_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, Routes.MESSAGE_CONTROLLER_NAME, Routes.EDIT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string UpdateTelegramUserReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, Routes.EDIT_ACTION_NAME);
        #endregion

        #region TelegramBot
        /// <inheritdoc/>
        public readonly static string GetBotUsernameReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.NAME_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SendTextMessageTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.TEXT_CONTROLLER_NAME, Routes.MESSAGE_CONTROLLER_NAME, Routes.SEND_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SetWebConfigReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.WEB_CONTROLLER_NAME, Routes.CONFIGURATION_CONTROLLER_NAME, Routes.EDIT_ACTION_NAME);
        #endregion

        #region Helpdesk
        /// <inheritdoc/>
        public readonly static string RubricForIssuesUpdateHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.RUBRIC_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricsForIssuesListHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRIC_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.LIST_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricForIssuesMoveHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRIC_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.MOVE_ACTION_NAME);


        /// <inheritdoc/>
        public readonly static string IssueUpdateHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, Routes.ISSUE_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string IssuesSelectHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-for-{Routes.USER_CONTROLLER_NAME}", Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string IssueGetHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-for-{Routes.USER_CONTROLLER_NAME}", Routes.READ_ACTION_NAME);


        /// <inheritdoc/>
        public readonly static string MessageOfIssueUpdateHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.MESSAGE_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string MessageOfIssueSetAsResponseHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.MESSAGE_CONTROLLER_NAME}", Routes.SET_ACTION_NAME, Routes.MARK_ACTION_NAME);
        #endregion

        #region storage cloud parameters
        /// <inheritdoc/>
        public readonly static string SaveCloudParameterReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.CLOUD_CONTROLLER_NAME, Routes.PROPERTY_CONTROLLER_NAME, Routes.SET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ReadCloudParameterReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.CLOUD_CONTROLLER_NAME, Routes.PROPERTY_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FindCloudParameterReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.CLOUD_CONTROLLER_NAME, Routes.PROPERTY_CONTROLLER_NAME, Routes.FIND_ACTION_NAME);
        #endregion
    }

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
        /// Helpdesk
        /// </summary>
        public const string HELPDESK_CONTROLLER_NAME = "helpdesk";

        /// <summary>
        /// Cloud
        /// </summary>
        public const string CLOUD_CONTROLLER_NAME = "cloud";

        /// <summary>
        /// Property
        /// </summary>
        public const string PROPERTY_CONTROLLER_NAME = "property";

        /// <summary>
        /// Form
        /// </summary>
        public const string FORM_CONTROLLER_NAME = "form";

        /// <summary>
        /// Disabled
        /// </summary>
        public const string DISABLED_CONTROLLER_NAME = "disabled";

        /// <summary>
        /// Issue
        /// </summary>
        public const string ISSUE_CONTROLLER_NAME = "issue";

        /// <summary>
        /// Theme
        /// </summary>
        public const string THEME_CONTROLLER_NAME = "theme";

        /// <summary>
        /// Rubric
        /// </summary>
        public const string RUBRIC_CONTROLLER_NAME = "rubric";

        /// <summary>
        /// Web
        /// </summary>
        public const string WEB_CONTROLLER_NAME = "web";

        /// <summary>
        /// main
        /// </summary>
        public const string MAIN_CONTROLLER_NAME = "telegram";

        /// <summary>
        /// configuration
        /// </summary>
        public const string CONFIGURATION_CONTROLLER_NAME = "configuration";

        /// <summary>
        /// Account
        /// </summary>
        public const string ACCOUNT_CONTROLLER_NAME = "account";

        /// <summary>
        /// User
        /// </summary>
        public const string USER_CONTROLLER_NAME = "user";

        /// <summary>
        /// Identity
        /// </summary>
        public const string IDENTITY_CONTROLLER_NAME = "identity";

        /// <summary>
        /// Name
        /// </summary>
        public const string NAME_CONTROLLER_NAME = "name";

        /// <summary>
        /// Text
        /// </summary>
        public const string TEXT_CONTROLLER_NAME = "text";

        /// <summary>
        /// Message
        /// </summary>
        public const string MESSAGE_CONTROLLER_NAME = "message";



        /// <summary>
        /// create
        /// </summary>
        public const string CREATE_ACTION_NAME = "create";

        /// <summary>
        /// add
        /// </summary>
        public const string ADD_ACTION_NAME = "add";

        /// <summary>
        /// update
        /// </summary>
        public const string UPDATE_ACTION_NAME = "update";

        /// <summary>
        /// Подключить/присоединить
        /// </summary>
        public const string JOIN_ACTION_NAME = "join";

        /// <summary>
        /// Show
        /// </summary>
        public const string SHOW_ACTION_NAME = "show";

        /// <summary>
        /// Mode
        /// </summary>
        public const string MODE_ACTION_NAME = "mode";

        /// <summary>
        /// Отправить
        /// </summary>
        public const string SEND_ACTION_NAME = "send";

        /// <summary>
        /// SET
        /// </summary>
        public const string SET_ACTION_NAME = "set";

        /// <summary>
        /// Mark
        /// </summary>
        public const string MARK_ACTION_NAME = "mark";

        /// <summary>
        /// Подтвердить
        /// </summary>
        public const string CONFIRM_ACTION_NAME = "confirm";

        /// <summary>
        /// Удалить
        /// </summary>
        public const string DELETE_ACTION_NAME = "delete";

        /// <summary>
        /// Прочитать
        /// </summary>
        public const string READ_ACTION_NAME = "read";

        /// <summary>
        /// Найти
        /// </summary>
        public const string FIND_ACTION_NAME = "find";

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
        /// Move
        /// </summary>
        public const string MOVE_ACTION_NAME = "move";

        /// <summary>
        /// Выборка
        /// </summary>
        public const string SELECT_ACTION_NAME = "select";

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
    /// Roles
    /// </summary>
    public static class Roles
    {
        /// <summary>
        /// admin
        /// </summary>
        public const string Admin = "admin";

        /// <summary>
        /// HelpDeskTelegramBotManager
        /// </summary>
        public const string HelpDeskTelegramBotManager = "HelpDeskTelegramBotManager";

        /// <summary>
        /// HelpDeskTelegramBotUnit
        /// </summary>
        public const string HelpDeskTelegramBotUnit = "HelpDeskTelegramBotUnit";
    }

    /// <summary>
    /// Имя пространства имён мемкеша для хранения сессий
    /// </summary>
    public const string SESSION_MEMCASHE_NAMESPASE = "sessions";

    /// <summary>
    /// Шаблон имени папки
    /// </summary>
    public const string FOLDER_NAME_TEMPLATE = @"^[a-zA-Z0-9_-]{2,128}$";

    /// <summary>
    /// Шаблон системного имени
    /// </summary>
    public const string SYSTEM_NAME_TEMPLATE = @"^[a-zA-Z][a-zA-Z0-9_]{1,72}[a-zA-Z0-9]{0,128}$";

    /// <summary>
    /// Сообщение для шаблона системного имени
    /// </summary>
    public const string SYSTEM_NAME_TEMPLATE_MESSAGE = "Системное имя не корректное. Оно может содержать латинские буквы и цифры. Первым символом должна идти буква. Минимум 2 символа!";
}