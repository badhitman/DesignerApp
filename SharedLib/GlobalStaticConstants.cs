////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;

namespace SharedLib;

/// <summary>
/// Константы
/// </summary>
public static partial class GlobalStaticConstants
{
    /// <summary>
    /// TinyMCEditorConf
    /// </summary>
    public static Dictionary<string, object> TinyMCEditorConf(string images_upload_url) => new()
        {
            { "menubar", true },
            { "plugins", "autolink media link image code emoticons table" },
            { "toolbar", "undo redo | styleselect styles | forecolor | bold italic underline | table | alignleft aligncenter alignright alignjustify | outdent indent | link image paste | code" },
            { "images_upload_credentials", true },
            { "paste_data_images", true },
            { "width", "100%" },
            { "automatic_uploads", true },
            { "file_picker_types", "file image media" },
            { "images_reuse_filename", true },
            { "images_upload_url", images_upload_url } };

    /// <summary>
    /// /lib/tinymce/tinymce.min.js
    /// </summary>
    public static readonly string TinyMCEditorScriptSrc = "/lib/tinymce/tinymce.min.js";

    /// <summary>
    /// JsonSerializerSettings
    /// </summary>
    public static JsonSerializerSettings JsonSerializerSettings => new() { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };


    static string? _initSalt;
    /// <summary>
    /// InitSalt
    /// </summary>
    public static string InitSalt
    {
        get
        {
            _initSalt ??= Guid.NewGuid().ToString();
            return _initSalt;
        }
    }

    /// <summary>
    /// Fake Host
    /// </summary>
    public const string FakeHost = "fake.null";

    /// <summary>
    /// TinyMCEditorUploadImage
    /// </summary>
    public const string TinyMCEditorUploadImage = "/TinyMCEditor/UploadImage/";

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
    /// OrderDocumentName
    /// </summary>
    public const string OrderDocumentName = "[OrderDocumentName]";

    /// <summary>
    /// OrderDocumentDate
    /// </summary>
    public const string OrderDocumentDate = "[OrderDocumentDate]";

    /// <summary>
    /// OrderStatusInfo
    /// </summary>
    public const string OrderStatusInfo = "[OrderStatusInfo]";

    /// <summary>
    /// HostAddress
    /// </summary>
    public const string HostAddress = "[HostAddress]";

    /// <summary>
    /// OrderLinkAddress
    /// </summary>
    public const string OrderLinkAddress = "[OrderLinkAddress]";

    /// <summary>
    /// Cloud storage metadata
    /// </summary>
    public static class CloudStorageMetadata
    {
        /// <summary>
        /// HomePagePublic
        /// </summary>
        public static StorageMetadataModel HomePagePublic => new()
        {
            ApplicationName = Path.Combine(Routes.HOME_CONTROLLER_NAME, Routes.PAGE_CONTROLLER_NAME),
            Name = Routes.PUBLIC_CONTROLLER_NAME,
        };

        /// <summary>
        /// HomePagePrivate
        /// </summary>
        public static StorageMetadataModel HomePagePrivate => new()
        {
            ApplicationName = Path.Combine(Routes.HOME_CONTROLLER_NAME, Routes.PAGE_CONTROLLER_NAME),
            Name = Routes.PRIVATE_CONTROLLER_NAME,
        };

        /// <summary>
        /// TitleMain
        /// </summary>
        public static StorageMetadataModel TitleMain => new()
        {
            ApplicationName = Routes.MAIN_CONTROLLER_NAME,
            Name = Routes.TITLE_CONTROLLER_NAME,
        };

        /// <summary>
        /// Отображение отключённых рубрик
        /// </summary>
        public static StorageMetadataModel ParameterShowDisabledRubrics => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = Path.Combine(Routes.RUBRIC_CONTROLLER_NAME, Routes.FORM_CONTROLLER_NAME, $"{Routes.SHOW_ACTION_NAME}-{Routes.DISABLED_CONTROLLER_NAME}"),
        };

        /// <summary>
        /// Режим выбора рубрик в заявках
        /// </summary>
        public static StorageMetadataModel ModeSelectingRubrics => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = Path.Combine(Routes.RUBRIC_CONTROLLER_NAME, Routes.FORM_CONTROLLER_NAME, $"{Routes.MODE_ACTION_NAME}-{Routes.SELECT_ACTION_NAME}"),
        };

        /// <summary>
        /// Включение командного режима боту
        /// </summary>
        public static StorageMetadataModel ParameterIsCommandModeTelegramBot => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = Path.Combine(Routes.TELEGRAM_CONTROLLER_NAME, Routes.COMMAND_CONTROLLER_NAME, Routes.MODE_CONTROLLER_NAME),
        };

        /// <summary>
        /// Отображать кнопку создания обращения
        /// </summary>
        public static StorageMetadataModel ShowCreatingIssue => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = Path.Combine(Routes.CREATE_ACTION_NAME, Routes.COMMAND_CONTROLLER_NAME, Routes.ADD_ACTION_NAME),
        };

        /// <summary>
        /// RubricIssueForCreateOrder
        /// </summary>
        public static StorageMetadataModel RubricIssueForCreateOrder => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = Path.Combine(Routes.CREATE_ACTION_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.RUBRIC_CONTROLLER_NAME),
        };

        /// <summary>
        /// Уведомления Telegram о созданных заявках
        /// </summary>
        public static StorageMetadataModel HelpdeskNotificationTelegramForCreateIssue => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = $"{Routes.TELEGRAM_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}",
            PrefixPropertyName = $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.CREATE_ACTION_NAME}",
        };

        /// <summary>
        /// Настройка пересылки входящих сообщений в TelegramBot: глобально для пользователей на которых нет персональных подписок
        /// </summary>
        public static StorageMetadataModel HelpdeskNotificationTelegramGlobalForIncomingMessage => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = $"{Routes.TELEGRAM_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.GLOBAL_CONTROLLER_NAME,
        };

        #region commerce notifications
        /// <summary>
        /// Тема уведомления при создании нового заказа
        /// </summary>
        public static StorageMetadataModel CommerceNewOrderSubjectNotification => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            Name = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.CREATE_ACTION_NAME}",
            PrefixPropertyName = Routes.SUBJECT_CONTROLLER_NAME
        };

        /// <summary>
        /// Текст уведомления при создании нового заказа
        /// </summary>
        public static StorageMetadataModel CommerceNewOrderBodyNotification => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            Name = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.CREATE_ACTION_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
        };


        /// <summary>
        /// Тема уведомления при изменении статуса заказа
        /// </summary>
        public static StorageMetadataModel CommerceStatusChangeOrderSubjectNotification(StatusesDocumentsEnum stage) => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            Name = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.STATUS_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.SUBJECT_CONTROLLER_NAME,
            OwnerPrimaryKey = (int)stage
        };

        /// <summary>
        /// Текст уведомления при изменении статуса заказа
        /// </summary>
        public static StorageMetadataModel CommerceStatusChangeOrderBodyNotification(StatusesDocumentsEnum stage) => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            Name = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.STATUS_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
            OwnerPrimaryKey = (int)stage
        };


        /// <summary>
        /// Тема уведомления при добавлении комментария к заказу
        /// </summary>
        public static StorageMetadataModel CommerceNewMessageOrderSubjectNotification => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            Name = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.MESSAGE_CONTROLLER_NAME}-{Routes.ADD_ACTION_NAME}",
            PrefixPropertyName = Routes.SUBJECT_CONTROLLER_NAME,
        };

        /// <summary>
        /// Текст уведомления при добавлении комментария к заказу
        /// </summary>
        public static StorageMetadataModel CommerceNewMessageOrderBodyNotification => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            Name = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.MESSAGE_CONTROLLER_NAME}-{Routes.ADD_ACTION_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
        };



        /// <summary>
        /// TELEGRAM: Текст уведомления при создании нового заказа
        /// </summary>
        public static StorageMetadataModel CommerceNewOrderBodyNotificationTelegram => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            Name = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.CREATE_ACTION_NAME}-{Routes.TELEGRAM_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
        };

        /// <summary>
        /// TELEGRAM: Текст уведомления при изменении статуса заказа
        /// </summary>
        public static StorageMetadataModel CommerceStatusChangeOrderBodyNotificationTelegram(StatusesDocumentsEnum stepEnum) => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            Name = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.STATUS_CONTROLLER_NAME}-{Routes.TELEGRAM_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
            OwnerPrimaryKey = (int)stepEnum
        };

        /// <summary>
        /// TELEGRAM: Текст уведомления при добавлении комментария к заказу
        /// </summary>
        public static StorageMetadataModel CommerceNewMessageOrderBodyNotificationTelegram => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            Name = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.MESSAGE_CONTROLLER_NAME}-{Routes.ADD_ACTION_NAME}-{Routes.TELEGRAM_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
        };
        #endregion

        /// <summary>
        /// Фильтр консоли по пользователю
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public static StorageMetadataModel ConsoleFilterForUser(string user_id) => new()
        {
            ApplicationName = Path.Combine(Routes.HELPDESK_CONTROLLER_NAME, Routes.CONSOLE_CONTROLLER_NAME),
            Name = Path.Combine(Routes.FILTER_CONTROLLER_NAME, user_id),
        };

        /// <summary>
        /// Уведомления в Telegram пользователю о событиях в его документах
        /// </summary>
        public static StorageMetadataModel NotificationTelegramForIssueUser(string user_id)
            => new()
            {
                ApplicationName = HelpdeskNotificationsTelegramAppName,
                Name = Routes.USER_CONTROLLER_NAME,
                PrefixPropertyName = user_id,
            };

        /// <summary>
        /// Фильтр консоли по пользователю
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public static StorageMetadataModel OrderCartForUser(string user_id) => new()
        {
            ApplicationName = Path.Combine(Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME),
            Name = Path.Combine(Routes.CART_CONTROLLER_NAME, user_id),
        };

        /// <summary>
        /// Переадресация для пользователя
        /// </summary>
        public static StorageMetadataModel HelpdeskNotificationsTelegramForUser(long chat_id) => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = $"{Routes.TELEGRAM_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}/{Routes.USER_CONTROLLER_NAME}",
            PrefixPropertyName = $"{chat_id}",
        };
    }

    /// <summary>
    /// Transmission MQ queues
    /// </summary>
    public static class TransmissionQueues
    {
        #region Web
        #region Identity
        /// <summary>
        /// Получить пользователей из Identity по их идентификаторам
        /// </summary>
        public readonly static string GetUsersOfIdentityReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.IDENTITY_CONTROLLER_NAME, Routes.USERS_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SelectUsersOfIdentityReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.IDENTITY_CONTROLLER_NAME, Routes.USERS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SetRoleForUserOfIdentityReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.IDENTITY_CONTROLLER_NAME, $"{Routes.USER_CONTROLLER_NAME}_{Routes.ROLE_CONTROLLER_NAME}", Routes.SET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetUsersOfIdentityByTelegramIdsReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.IDENTITY_CONTROLLER_NAME, Routes.USERS_CONTROLLER_NAME, $"{Routes.GET_ACTION_NAME}-by-{Routes.TELEGRAM_CONTROLLER_NAME}");
        #endregion

        /// <inheritdoc/>
        public readonly static string SendEmailReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.EMAIL_CONTROLLER_NAME, Routes.OUTGOING_CONTROLLER_NAME, Routes.SEND_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetTelegramUserReceive = Path.Combine(TransmissionQueueNamePrefix, $"{Routes.WEB_CONTROLLER_NAME}_{Routes.TELEGRAM_CONTROLLER_NAME}", $"{Routes.USER_CONTROLLER_NAME}_{Routes.CACHE_CONTROLLER_NAME}", Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetWebConfigReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.WEB_CONTROLLER_NAME, Routes.CONFIGURATION_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string TelegramJoinAccountConfirmReceive = Path.Combine(TransmissionQueueNamePrefix, $"{Routes.WEB_CONTROLLER_NAME}_{Routes.TELEGRAM_CONTROLLER_NAME}", $"{Routes.JOIN_ACTION_NAME}_{Routes.ACCOUNT_CONTROLLER_NAME}", Routes.CONFIRM_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string TelegramJoinAccountDeleteReceive = Path.Combine(TransmissionQueueNamePrefix, $"{Routes.WEB_CONTROLLER_NAME}_{Routes.TELEGRAM_CONTROLLER_NAME}", $"{Routes.JOIN_ACTION_NAME}_{Routes.ACCOUNT_CONTROLLER_NAME}", Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string UpdateTelegramMainUserMessageReceive = Path.Combine(TransmissionQueueNamePrefix, $"{Routes.WEB_CONTROLLER_NAME}_{Routes.TELEGRAM_CONTROLLER_NAME}", $"{Routes.MAIN_CONTROLLER_NAME}_{Routes.MESSAGE_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string UpdateTelegramUserReceive = Path.Combine(TransmissionQueueNamePrefix, $"{Routes.WEB_CONTROLLER_NAME}_{Routes.TELEGRAM_CONTROLLER_NAME}", $"{Routes.USER_CONTROLLER_NAME}_{Routes.CACHE_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);
        #endregion

        #region TelegramBot
        /// <inheritdoc/>
        public readonly static string GetBotUsernameReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.NAME_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SendTextMessageTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, $"{Routes.TEXT_CONTROLLER_NAME}_{Routes.MESSAGE_CONTROLLER_NAME}", Routes.SEND_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ForwardTextMessageTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, $"{Routes.TEXT_CONTROLLER_NAME}_{Routes.MESSAGE_CONTROLLER_NAME}", Routes.FORWARD_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ReadFileTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.FILE_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ChatsReadTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.CHATS_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ChatReadTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.CHAT_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ChatsFindForUserTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, $"{Routes.CHATS_CONTROLLER_NAME}-for-{Routes.USERS_CONTROLLER_NAME}", Routes.FIND_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string MessagesChatsSelectTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, $"{Routes.MESSAGES_CONTROLLER_NAME}-of-{Routes.CHATS_CONTROLLER_NAME}", Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ChatsSelectTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.CHATS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ErrorsForChatsSelectTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, $"{Routes.ERRORS_CONTROLLER_NAME}-of-{Routes.CHATS_CONTROLLER_NAME}", Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SetWebConfigReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, $"{Routes.WEB_CONTROLLER_NAME}_{Routes.CONFIGURATION_CONTROLLER_NAME}", Routes.SET_ACTION_NAME);
        #endregion

        #region Constructor
        #region projects
        /// <inheritdoc/>
        public readonly static string ProjectsReadReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.PROJECTS_CONTROLLER_NAME, Routes.DATA_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ProjectsForUserReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.PROJECTS_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, $"{Routes.GET_ACTION_NAME}_{Routes.ROWS_CONTROLLER_NAME}");

        /// <inheritdoc/>
        public readonly static string ProjectCreateReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.PROJECT_CONTROLLER_NAME, Routes.CREATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SetMarkerDeleteProjectReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.PROJECT_CONTROLLER_NAME, $"{Routes.MARK_ACTION_NAME}_{Routes.DELETE_ACTION_NAME}", Routes.SET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetMembersOfProjectReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.PROJECT_CONTROLLER_NAME, Routes.MEMBERS_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string DeleteMembersFromProjectReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.PROJECT_CONTROLLER_NAME, Routes.MEMBERS_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string CanEditProjectReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.PROJECT_CONTROLLER_NAME, Routes.MEMBER_CONTROLLER_NAME, $"{Routes.ALLOW_ACTION_NAME}_{Routes.EDIT_ACTION_NAME}_{Routes.CHECK_ACTION_NAME}");

        /// <inheritdoc/>
        public readonly static string AddMembersToProjectReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.PROJECT_CONTROLLER_NAME, Routes.MEMBERS_CONTROLLER_NAME, Routes.ADD_ACTION_NAME);
        #endregion

        #region directories
        /// <inheritdoc/>
        public readonly static string ReadDirectoriesReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORIES_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string UpdateOrCreateDirectoryReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORY_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetDirectoriesReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORIES_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string DeleteDirectoryReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORY_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetElementsOfDirectoryReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORY_CONTROLLER_NAME, Routes.ELEMENTS_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string CreateElementForDirectoryReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORY_CONTROLLER_NAME, Routes.ELEMENT_CONTROLLER_NAME, Routes.CREATE_ACTION_NAME);
        #endregion

        #region forms
        /// <inheritdoc/>
        public readonly static string SelectFormsReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.FORMS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetFormReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.FORM_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FormUpdateOrCreateReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.FORM_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FormDeleteReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.FORM_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FieldFormMoveReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.FORM_CONTROLLER_NAME, Routes.FIELD_CONTROLLER_NAME, Routes.MOVE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FieldDirectoryFormMoveReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.FORM_CONTROLLER_NAME, $"{Routes.FIELD_CONTROLLER_NAME}_{Routes.DIRECTORY_CONTROLLER_NAME}", Routes.MOVE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FormFieldUpdateOrCreateReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.FORM_CONTROLLER_NAME, Routes.FIELD_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FormFieldDeleteReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.FORM_CONTROLLER_NAME, Routes.FIELD_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FormFieldDirectoryUpdateOrCreateReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.FORM_CONTROLLER_NAME, $"{Routes.FIELD_CONTROLLER_NAME}_{Routes.DIRECTORY_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FormFieldDirectoryDeleteReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.FORM_CONTROLLER_NAME, $"{Routes.FIELD_CONTROLLER_NAME}_{Routes.DIRECTORY_CONTROLLER_NAME}", Routes.DELETE_ACTION_NAME);
        #endregion

        /// <inheritdoc/>
        public readonly static string GetSessionDocumentDataReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSION_CONTROLLER_NAME, Routes.DOCUMENT_CONTROLLER_NAME, $"{Routes.DATA_CONTROLLER_NAME}_{Routes.GET_ACTION_NAME}");

        /// <inheritdoc/>
        public readonly static string SetValueFieldSessionDocumentDataReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSION_CONTROLLER_NAME, Routes.DOCUMENT_CONTROLLER_NAME, Routes.DATA_CONTROLLER_NAME, Routes.VALUE_CONTROLLER_NAME, Routes.FIELD_CONTROLLER_NAME, Routes.SET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SetDoneSessionDocumentDataReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSION_CONTROLLER_NAME, Routes.DOCUMENT_CONTROLLER_NAME, Routes.DATA_CONTROLLER_NAME, Routes.STATUS_CONTROLLER_NAME, Routes.DONE_ACTION_NAME, Routes.SET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string DeleteValuesFieldsByGroupSessionDocumentDataByRowNumReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSION_CONTROLLER_NAME, Routes.DOCUMENT_CONTROLLER_NAME, Routes.DATA_CONTROLLER_NAME, Routes.ROW_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string CheckAndNormalizeSortIndexForElementsOfDirectoryReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORY_CONTROLLER_NAME, Routes.CHECK_ACTION_NAME, Routes.NORMALIZE_ACTION_NAME, Routes.SORT_ACTION_NAME, Routes.INDEX_CONTROLLER_NAME, Routes.ELEMENTS_CONTROLLER_NAME);

        /// <inheritdoc/>
        public readonly static string UpdateOrCreateDocumentSchemeReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DOCUMENT_CONTROLLER_NAME, Routes.SCHEME_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RequestDocumentsSchemesReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DOCUMENTS_CONTROLLER_NAME, Routes.SCHEMES_CONTROLLER_NAME, Routes.REQUEST_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetDocumentSchemeReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DOCUMENTS_CONTROLLER_NAME, Routes.SCHEMES_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string DeleteDocumentSchemeReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DOCUMENT_CONTROLLER_NAME, Routes.SCHEME_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string CreateOrUpdateTabOfDocumentSchemeReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DOCUMENT_CONTROLLER_NAME, $"{Routes.SCHEME_CONTROLLER_NAME}_{Routes.TAB_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string MoveTabOfDocumentSchemeReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DOCUMENT_CONTROLLER_NAME, $"{Routes.SCHEME_CONTROLLER_NAME}_{Routes.TAB_CONTROLLER_NAME}", Routes.MOVE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetTabOfDocumentSchemeReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DOCUMENT_CONTROLLER_NAME, $"{Routes.SCHEME_CONTROLLER_NAME}_{Routes.TAB_CONTROLLER_NAME}", Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string DeleteTabOfDocumentSchemeReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DOCUMENT_CONTROLLER_NAME, $"{Routes.SCHEME_CONTROLLER_NAME}_{Routes.TAB_CONTROLLER_NAME}", Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetTabDocumentSchemeJoinFormReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DOCUMENT_CONTROLLER_NAME, $"{Routes.SCHEME_CONTROLLER_NAME}_{Routes.TAB_CONTROLLER_NAME}", Routes.JOIN_ACTION_NAME, Routes.FORM_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string CreateOrUpdateTabDocumentSchemeJoinFormReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DOCUMENT_CONTROLLER_NAME, $"{Routes.SCHEME_CONTROLLER_NAME}_{Routes.TAB_CONTROLLER_NAME}", Routes.JOIN_ACTION_NAME, Routes.FORM_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string MoveTabDocumentSchemeJoinFormReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DOCUMENT_CONTROLLER_NAME, $"{Routes.SCHEME_CONTROLLER_NAME}_{Routes.TAB_CONTROLLER_NAME}", Routes.JOIN_ACTION_NAME, Routes.FORM_CONTROLLER_NAME, Routes.MOVE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string DeleteTabDocumentSchemeJoinFormReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DOCUMENT_CONTROLLER_NAME, $"{Routes.SCHEME_CONTROLLER_NAME}_{Routes.TAB_CONTROLLER_NAME}", Routes.JOIN_ACTION_NAME, Routes.FORM_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SaveSessionFormReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSION_CONTROLLER_NAME, $"{Routes.DOCUMENT_CONTROLLER_NAME}_{Routes.FORM_CONTROLLER_NAME}", Routes.SAVE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SetStatusSessionDocumentReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSION_CONTROLLER_NAME, Routes.DOCUMENT_CONTROLLER_NAME, Routes.STATUS_CONTROLLER_NAME, Routes.SET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetSessionDocumentReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSION_CONTROLLER_NAME, Routes.DOCUMENT_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string AddRowToTableReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSION_CONTROLLER_NAME, Routes.DOCUMENT_CONTROLLER_NAME, Routes.ROW_CONTROLLER_NAME, Routes.ADD_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string UpdateOrCreateSessionDocumentReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSION_CONTROLLER_NAME, Routes.DOCUMENT_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RequestSessionsDocumentsReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSIONS_CONTROLLER_NAME, Routes.DOCUMENTS_CONTROLLER_NAME, Routes.REQUEST_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FindSessionsDocumentsByFormFieldNameReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSIONS_CONTROLLER_NAME, Routes.DOCUMENTS_CONTROLLER_NAME, Routes.FIND_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ClearValuesForFieldNameReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSION_CONTROLLER_NAME, Routes.DOCUMENT_CONTROLLER_NAME, Routes.FIELDS_CONTROLLER_NAME, Routes.NAME_CONTROLLER_NAME, Routes.VALUES_CONTROLLER_NAME, Routes.CLEAR_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string DeleteSessionDocumentReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.SESSION_CONTROLLER_NAME, Routes.DOCUMENT_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string UpdateElementOfDirectoryReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORY_CONTROLLER_NAME, Routes.ELEMENT_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetElementOfDirectoryReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORY_CONTROLLER_NAME, Routes.ELEMENT_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string DeleteElementFromDirectoryReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORY_CONTROLLER_NAME, Routes.ELEMENT_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string UpMoveElementOfDirectoryReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORY_CONTROLLER_NAME, Routes.ELEMENT_CONTROLLER_NAME, Routes.MOVE_ACTION_NAME, Routes.UP_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string DownMoveElementOfDirectoryReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORY_CONTROLLER_NAME, Routes.ELEMENT_CONTROLLER_NAME, Routes.MOVE_ACTION_NAME, Routes.DOWN_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetDirectoryReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.DIRECTORY_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SetProjectAsMainReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.PROJECT_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, Routes.MAIN_CONTROLLER_NAME, Routes.SET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetCurrentMainProjectReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.PROJECT_CONTROLLER_NAME, Routes.MAIN_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string UpdateProjectReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.PROJECT_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);
        #endregion

        #region Commerce
        /// <inheritdoc/>
        public readonly static string ReadFileCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.FILE_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string StatusChangeOrderReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.STATUS_CONTROLLER_NAME}-for-{Routes.ORDER_CONTROLLER_NAME}", Routes.CHANGE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GoodsReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.GOODS_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GoodsSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.GOODS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrdersSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDERS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrdersReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDERS_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationsSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationsFindCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.FIND_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationsReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATION_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string AddressesOrganizationsReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ADDRESSES_CONTROLLER_NAME, Routes.ORGANIZATION_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationUpdateOrCreateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationSetLegalCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.LEGAL_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string AddressOrganizationUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.ADDRESS_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <summary>
        /// Обновление номенклатуры
        /// </summary>
        public readonly static string GoodsUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.GOODS_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrderUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <summary>
        /// Прикрепить файл к заказу (счёт, акт и т.п.)
        /// </summary>
        public readonly static string AttachmentAddToOrderCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.ATTACHMENT_ACTION_NAME, Routes.ADD_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string AttachmentDeleteFromOrderCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.ATTACHMENT_ACTION_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RowForOrderUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.ROW_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string PaymentDocumentUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.PAYMENT_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string PaymentDocumentDeleteCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.PAYMENT_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RowsDeleteFromOrderCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.ROWS_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OfferUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.GOODS_CONTROLLER_NAME, Routes.OFFER_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OfferSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.GOODS_CONTROLLER_NAME, Routes.OFFER_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OfferReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.GOODS_CONTROLLER_NAME, Routes.OFFER_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string PricesRulesGetForOfferCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.PRICE_CONTROLLER_NAME, Routes.OFFER_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string PriceRuleUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.PRICE_CONTROLLER_NAME, Routes.OFFER_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string AddressOrganizationDeleteCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.ADDRESS_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <summary>
        /// Удалить оффер
        /// </summary>
        public readonly static string OfferDeleteCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.OFFER_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string PriceRuleDeleteCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.PRICE_CONTROLLER_NAME, Routes.OFFER_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        #endregion

        #region Helpdesk
        /// <inheritdoc/>
        public readonly static string RubricForIssuesUpdateHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.RUBRIC_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ArticleUpdateHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, Routes.ARTICLE_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string TagArticleSetReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.TAG_CONTROLLER_NAME}-of-{Routes.ARTICLE_CONTROLLER_NAME}", Routes.SET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricsForArticleSetReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRICS_CONTROLLER_NAME}-for-{Routes.ARTICLES_CONTROLLER_NAME}", Routes.SET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricsForIssuesListHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRIC_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.LIST_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricForIssuesMoveHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRIC_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.MOVE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricForIssuesReadHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRIC_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.READ_ACTION_NAME);


        /// <inheritdoc/>
        public readonly static string IssueUpdateHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, Routes.ISSUE_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string IssuesSelectHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, Routes.ISSUE_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ArticlesSelectHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, Routes.ARTICLES_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string TagsOfArticlesSelectReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.TAGS_CONTROLLER_NAME}-of-{Routes.ARTICLES_CONTROLLER_NAME}", Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ArticlesReadReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, Routes.ARTICLES_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ConsoleIssuesSelectHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.CONSOLE_CONTROLLER_NAME}", Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string StatusChangeIssueHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.STATUS_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.CHANGE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string PulseIssuePushHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, Routes.ISSUE_CONTROLLER_NAME, Routes.PULSE_CONTROLLER_NAME, Routes.PUSH_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string IssuesGetHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUES_CONTROLLER_NAME}-for-{Routes.USER_CONTROLLER_NAME}", Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string IncomingTelegramMessageHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-for-{Routes.TELEGRAM_CONTROLLER_NAME}", $"{Routes.MESSAGE_CONTROLLER_NAME}-{Routes.INCOMING_CONTROLLER_NAME}");

        /// <inheritdoc/>
        public readonly static string SubscribeIssueUpdateHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.SUBSCRIBE_CONTROLLER_NAME}-{Routes.ISSUE_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SubscribesIssueListHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.SUBSCRIBE_CONTROLLER_NAME}-{Routes.ISSUE_CONTROLLER_NAME}", Routes.LIST_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string PulseJournalHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.PULSE_CONTROLLER_NAME}-{Routes.JOURNAL_CONTROLLER_NAME}", Routes.LIST_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ExecuterIssueUpdateHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.EXECUTER_CONTROLLER_NAME}-{Routes.ISSUE_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);


        /// <inheritdoc/>
        public readonly static string MessageOfIssueUpdateHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.MESSAGE_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string MessageOfIssueVoteHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.MESSAGE_CONTROLLER_NAME}", Routes.VOTE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string MessagesOfIssueListHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.MESSAGE_CONTROLLER_NAME}", Routes.LIST_ACTION_NAME);
        #endregion

        #region Storage
        /// <inheritdoc/>
        public readonly static string FilesSelectReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.STORAGE_CONTROLLER_NAME, Routes.FILES_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FilesAreaGetMetadataReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.STORAGE_CONTROLLER_NAME, $"{Routes.FILES_CONTROLLER_NAME}-{Routes.AREAS_CONTROLLER_NAME}", $"{Routes.GET_ACTION_NAME}-{Routes.METADATA_CONTROLLER_NAME}");

        /// <inheritdoc/>
        public readonly static string SaveCloudParameterReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.CLOUD_CONTROLLER_NAME, Routes.PROPERTY_CONTROLLER_NAME, Routes.SET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SaveFileReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.CLOUD_CONTROLLER_NAME, Routes.FILE_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ReadFileReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.CLOUD_CONTROLLER_NAME, Routes.FILE_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ReadCloudParameterReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.CLOUD_CONTROLLER_NAME, Routes.PROPERTY_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FindCloudParameterReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.CLOUD_CONTROLLER_NAME, Routes.PROPERTY_CONTROLLER_NAME, Routes.FIND_ACTION_NAME);
        #endregion
    }

    /// <summary>
    /// TelegramIdClaimName
    /// </summary>
    public readonly static string TelegramIdClaimName = Path.Combine(Routes.USER_CONTROLLER_NAME, Routes.TELEGRAM_CONTROLLER_NAME, Routes.IDENTITY_CONTROLLER_NAME);

    /// <summary>
    /// имена контроллеров и действий
    /// </summary>
    public static class Routes
    {
        /// <summary>
        /// Global
        /// </summary>
        public const string GLOBAL_CONTROLLER_NAME = "global";

        /// <summary>
        /// Аутентификация
        /// </summary>
        public const string AUTHENTICATION_CONTROLLER_NAME = "authentication";

        /// <summary>
        /// Пользователи
        /// </summary>
        public const string USERS_CONTROLLER_NAME = "users";

        /// <summary>
        /// Notes
        /// </summary>
        public const string NOTES_CONTROLLER_NAME = "notes";

        /// <summary>
        /// Address
        /// </summary>
        public const string ADDRESS_CONTROLLER_NAME = "address";

        /// <summary>
        /// Articles
        /// </summary>
        public const string ARTICLES_CONTROLLER_NAME = "articles";

        /// <summary>
        /// Tags
        /// </summary>
        public const string TAGS_CONTROLLER_NAME = "tags";

        /// <summary>
        /// Tag
        /// </summary>
        public const string TAG_CONTROLLER_NAME = "tag";

        /// <summary>
        /// Article
        /// </summary>
        public const string ARTICLE_CONTROLLER_NAME = "article";

        /// <summary>
        /// Goods
        /// </summary>
        public const string GOODS_CONTROLLER_NAME = "goods";

        /// <summary>
        /// Orders
        /// </summary>
        public const string ORDERS_CONTROLLER_NAME = "orders";

        /// <summary>
        /// Order
        /// </summary>
        public const string ORDER_CONTROLLER_NAME = "order";

        /// <summary>
        /// Subject
        /// </summary>
        public const string SUBJECT_CONTROLLER_NAME = "subject";

        /// <summary>
        /// Body
        /// </summary>
        public const string BODY_CONTROLLER_NAME = "body";

        /// <summary>
        /// Payment
        /// </summary>
        public const string PAYMENT_CONTROLLER_NAME = "payment";

        /// <summary>
        /// Payments
        /// </summary>
        public const string PAYMENTS_CONTROLLER_NAME = "payments";

        /// <summary>
        /// Row
        /// </summary>
        public const string ROW_CONTROLLER_NAME = "row";

        /// <summary>
        /// Stage
        /// </summary>
        public const string STAGE_CONTROLLER_NAME = "stage";

        /// <summary>
        /// Rows
        /// </summary>
        public const string ROWS_CONTROLLER_NAME = "rows";

        /// <summary>
        /// Offer
        /// </summary>
        public const string OFFER_CONTROLLER_NAME = "offer";

        /// <summary>
        /// Offers
        /// </summary>
        public const string OFFERS_CONTROLLER_NAME = "offers";

        /// <summary>
        /// Notifications
        /// </summary>
        public const string NOTIFICATIONS_CONTROLLER_NAME = "notifications";

        /// <summary>
        /// Перечисления (enum`s)
        /// </summary>
        public const string ENUMS_CONTROLLER_NAME = "enums";

        /// <summary>
        /// Документы
        /// </summary>
        public const string DOCUMENTS_CONTROLLER_NAME = "documents";

        /// <summary>
        /// Документ
        /// </summary>
        public const string DOCUMENT_CONTROLLER_NAME = "document";

        /// <summary>
        /// Fields
        /// </summary>
        public const string FIELDS_CONTROLLER_NAME = "fields";

        /// <summary>
        /// Session
        /// </summary>
        public const string SESSION_CONTROLLER_NAME = "session";

        /// <summary>
        /// Sessions
        /// </summary>
        public const string SESSIONS_CONTROLLER_NAME = "sessions";

        /// <summary>
        /// Values
        /// </summary>
        public const string VALUES_CONTROLLER_NAME = "values";

        /// <summary>
        /// Value
        /// </summary>
        public const string VALUE_CONTROLLER_NAME = "value";

        /// <summary>
        /// Схема
        /// </summary>
        public const string SCHEME_CONTROLLER_NAME = "scheme";

        /// <summary>
        /// Tab
        /// </summary>
        public const string TAB_CONTROLLER_NAME = "tab";

        /// <summary>
        /// Схемы
        /// </summary>
        public const string SCHEMES_CONTROLLER_NAME = "schemes";

        /// <summary>
        /// Проекты
        /// </summary>
        public const string PROJECTS_CONTROLLER_NAME = "projects";

        /// <summary>
        /// Проект
        /// </summary>
        public const string PROJECT_CONTROLLER_NAME = "project";

        /// <summary>
        /// Справочники
        /// </summary>
        public const string DIRECTORIES_CONTROLLER_NAME = "directories";

        /// <summary>
        /// Members
        /// </summary>
        public const string MEMBERS_CONTROLLER_NAME = "members";

        /// <summary>
        /// Member
        /// </summary>
        public const string MEMBER_CONTROLLER_NAME = "member";

        /// <summary>
        /// Incoming Telegram message
        /// </summary>
        public const string INCOMING_CONTROLLER_NAME = "incoming";

        /// <summary>
        /// Telegram check-user
        /// </summary>
        public const string TELEGRAM_CONTROLLER_NAME = "telegram";

        /// <summary>
        /// Mode
        /// </summary>
        public const string MODE_CONTROLLER_NAME = "mode";

        /// <summary>
        /// Command
        /// </summary>
        public const string COMMAND_CONTROLLER_NAME = "command";

        /// <summary>
        /// Directory
        /// </summary>
        public const string DIRECTORY_CONTROLLER_NAME = "directory";

        /// <summary>
        /// Elements
        /// </summary>
        public const string ELEMENTS_CONTROLLER_NAME = "elements";

        /// <summary>
        /// Element
        /// </summary>
        public const string ELEMENT_CONTROLLER_NAME = "element";

        /// <summary>
        /// Data
        /// </summary>
        public const string DATA_CONTROLLER_NAME = "data";

        /// <summary>
        /// Filter
        /// </summary>
        public const string FILTER_CONTROLLER_NAME = "filter";

        /// <summary>
        /// Helpdesk
        /// </summary>
        public const string HELPDESK_CONTROLLER_NAME = "helpdesk";

        /// <summary>
        /// Cart
        /// </summary>
        public const string CART_CONTROLLER_NAME = "cart";

        /// <summary>
        /// Commerce
        /// </summary>
        public const string COMMERCE_CONTROLLER_NAME = "commerce";

        /// <summary>
        /// Organization
        /// </summary>
        public const string ORGANIZATION_CONTROLLER_NAME = "organization";

        /// <summary>
        /// Addresses
        /// </summary>
        public const string ADDRESSES_CONTROLLER_NAME = "addresses";

        /// <summary>
        /// Organizations
        /// </summary>
        public const string ORGANIZATIONS_CONTROLLER_NAME = "organizations";

        /// <summary>
        /// Delivery
        /// </summary>
        public const string DELIVERY_CONTROLLER_NAME = "delivery";

        /// <summary>
        /// Legal
        /// </summary>
        public const string LEGAL_CONTROLLER_NAME = "legal";

        /// <summary>
        /// Storage
        /// </summary>
        public const string STORAGE_CONTROLLER_NAME = "storage";

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
        /// Field
        /// </summary>
        public const string FIELD_CONTROLLER_NAME = "field";

        /// <summary>
        /// Forms
        /// </summary>
        public const string FORMS_CONTROLLER_NAME = "forms";

        /// <summary>
        /// Default
        /// </summary>
        public const string DEFAULT_CONTROLLER_NAME = "default";

        /// <summary>
        /// Price
        /// </summary>
        public const string PRICE_CONTROLLER_NAME = "price";

        /// <summary>
        /// Disabled
        /// </summary>
        public const string DISABLED_CONTROLLER_NAME = "disabled";

        /// <summary>
        /// Issue
        /// </summary>
        public const string ISSUE_CONTROLLER_NAME = "issue";

        /// <summary>
        /// Issues
        /// </summary>
        public const string ISSUES_CONTROLLER_NAME = "issues";

        /// <summary>
        /// Pulse
        /// </summary>
        public const string PULSE_CONTROLLER_NAME = "pulse";

        /// <summary>
        /// Journal
        /// </summary>
        public const string JOURNAL_CONTROLLER_NAME = "journal";

        /// <summary>
        /// Status
        /// </summary>
        public const string STATUS_CONTROLLER_NAME = "status";

        /// <summary>
        /// Index
        /// </summary>
        public const string INDEX_CONTROLLER_NAME = "index";

        /// <summary>
        /// Subscribe
        /// </summary>
        public const string SUBSCRIBE_CONTROLLER_NAME = "subscribe";

        /// <summary>
        /// Executer
        /// </summary>
        public const string EXECUTER_CONTROLLER_NAME = "executer";

        /// <summary>
        /// Theme 
        /// </summary>
        public const string THEME_CONTROLLER_NAME = "theme";

        /// <summary>
        /// Rubric
        /// </summary>
        public const string RUBRIC_CONTROLLER_NAME = "rubric";

        /// <summary>
        /// Rubrics
        /// </summary>
        public const string RUBRICS_CONTROLLER_NAME = "rubrics";

        /// <summary>
        /// Web
        /// </summary>
        public const string WEB_CONTROLLER_NAME = "web";

        /// <summary>
        /// main
        /// </summary>
        public const string MAIN_CONTROLLER_NAME = "main";

        /// <summary>
        /// Page
        /// </summary>
        public const string PAGE_CONTROLLER_NAME = "page";

        /// <summary>
        /// Home
        /// </summary>
        public const string HOME_CONTROLLER_NAME = "home";

        /// <summary>
        /// Authorize
        /// </summary>
        public const string AUTHORIZE_CONTROLLER_NAME = "authorize";

        /// <summary>
        /// Public
        /// </summary>
        public const string PUBLIC_CONTROLLER_NAME = "public";

        /// <summary>
        /// Private
        /// </summary>
        public const string PRIVATE_CONTROLLER_NAME = "private";

        /// <summary>
        /// Title
        /// </summary>
        public const string TITLE_CONTROLLER_NAME = "title";

        /// <summary>
        /// configuration
        /// </summary>
        public const string CONFIGURATION_CONTROLLER_NAME = "configuration";

        /// <summary>
        /// Description
        /// </summary>
        public const string DESCRIPTION_CONTROLLER_NAME = "description";

        /// <summary>
        /// Account
        /// </summary>
        public const string ACCOUNT_CONTROLLER_NAME = "account";

        /// <summary>
        /// User
        /// </summary>
        public const string USER_CONTROLLER_NAME = "user";

        /// <summary>
        /// Cache
        /// </summary>
        public const string CACHE_CONTROLLER_NAME = "cache";

        /// <summary>
        /// Role
        /// </summary>
        public const string ROLE_CONTROLLER_NAME = "role";

        /// <summary>
        /// Console
        /// </summary>
        public const string CONSOLE_CONTROLLER_NAME = "console";

        /// <summary>
        /// Constructor
        /// </summary>
        public const string CONSTRUCTOR_CONTROLLER_NAME = "constructor";

        /// <summary>
        /// Size
        /// </summary>
        public const string SIZE_CONTROLLER_NAME = "size";

        /// <summary>
        /// Identity
        /// </summary>
        public const string IDENTITY_CONTROLLER_NAME = "identity";

        /// <summary>
        /// E-Mail
        /// </summary>
        public const string EMAIL_CONTROLLER_NAME = "email";

        /// <summary>
        /// Outgoing
        /// </summary>
        public const string OUTGOING_CONTROLLER_NAME = "outgoing";

        /// <summary>
        /// Name
        /// </summary>
        public const string NAME_CONTROLLER_NAME = "name";

        /// <summary>
        /// Text
        /// </summary>
        public const string TEXT_CONTROLLER_NAME = "text";

        /// <summary>
        /// File
        /// </summary>
        public const string FILE_CONTROLLER_NAME = "file";

        /// <summary>
        /// Files
        /// </summary>
        public const string FILES_CONTROLLER_NAME = "files";

        /// <summary>
        /// Areas
        /// </summary>
        public const string AREAS_CONTROLLER_NAME = "areas";

        /// <summary>
        /// Metadata
        /// </summary>
        public const string METADATA_CONTROLLER_NAME = "metadata";

        /// <summary>
        /// Mongo
        /// </summary>
        public const string MONGO_CONTROLLER_NAME = "mongo";

        /// <summary>
        /// Chat
        /// </summary>
        public const string CHAT_CONTROLLER_NAME = "chat";

        /// <summary>
        /// Chats
        /// </summary>
        public const string CHATS_CONTROLLER_NAME = "chats";

        /// <summary>
        /// Errors
        /// </summary>
        public const string ERRORS_CONTROLLER_NAME = "errors";

        /// <summary>
        /// Message
        /// </summary>
        public const string MESSAGE_CONTROLLER_NAME = "message";

        /// <summary>
        /// Messages
        /// </summary>
        public const string MESSAGES_CONTROLLER_NAME = "messages";



        /// <summary>
        /// create
        /// </summary>
        public const string CREATE_ACTION_NAME = "create";

        /// <summary>
        /// add
        /// </summary>
        public const string ADD_ACTION_NAME = "add";

        /// <summary>
        /// Allow
        /// </summary>
        public const string ALLOW_ACTION_NAME = "allow";

        /// <summary>
        /// Update
        /// </summary>
        public const string UPDATE_ACTION_NAME = "update";

        /// <summary>
        /// Request
        /// </summary>
        public const string REQUEST_ACTION_NAME = "request";

        /// <summary>
        /// Attachment
        /// </summary>
        public const string ATTACHMENT_ACTION_NAME = "attachment";

        /// <summary>
        /// Image
        /// </summary>
        public const string IMAGE_ACTION_NAME = "image";

        /// <summary>
        /// Подключить/присоединить
        /// </summary>
        public const string JOIN_ACTION_NAME = "join";

        /// <summary>
        /// Save
        /// </summary>
        public const string SAVE_ACTION_NAME = "save";

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
        /// Переслать
        /// </summary>
        public const string FORWARD_ACTION_NAME = "forward";

        /// <summary>
        /// SET
        /// </summary>
        public const string SET_ACTION_NAME = "set";

        /// <summary>
        /// Done
        /// </summary>
        public const string DONE_ACTION_NAME = "done";

        /// <summary>
        /// Clear
        /// </summary>
        public const string CLEAR_ACTION_NAME = "clear";

        /// <summary>
        /// Mark
        /// </summary>
        public const string MARK_ACTION_NAME = "mark";

        /// <summary>
        /// Vote
        /// </summary>
        public const string VOTE_ACTION_NAME = "vote";

        /// <summary>
        /// Подтвердить
        /// </summary>
        public const string CONFIRM_ACTION_NAME = "confirm";

        /// <summary>
        /// Удалить
        /// </summary>
        public const string DELETE_ACTION_NAME = "delete";

        /// <summary>
        /// Поднять
        /// </summary>
        public const string UP_ACTION_NAME = "up";

        /// <summary>
        /// Опустить
        /// </summary>
        public const string DOWN_ACTION_NAME = "down";

        /// <summary>
        /// Прочитать
        /// </summary>
        public const string READ_ACTION_NAME = "read";

        /// <summary>
        /// Найти
        /// </summary>
        public const string FIND_ACTION_NAME = "find";

        /// <summary>
        /// Get
        /// </summary>
        public const string GET_ACTION_NAME = "get";

        /// <summary>
        /// Редактировать
        /// </summary>
        public const string EDIT_ACTION_NAME = "edit";

        /// <summary>
        /// Check
        /// </summary>
        public const string CHECK_ACTION_NAME = "check";

        /// <summary>
        /// Normalize
        /// </summary>
        public const string NORMALIZE_ACTION_NAME = "normalize";

        /// <summary>
        /// Sort
        /// </summary>
        public const string SORT_ACTION_NAME = "sort";

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
        /// Изменение
        /// </summary>
        public const string CHANGE_ACTION_NAME = "change";

        /// <summary>
        /// Push
        /// </summary>
        public const string PUSH_ACTION_NAME = "push";

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
        /// Debug
        /// </summary>
        public const string Debug = "debug";

        /// <summary>
        /// system
        /// </summary>
        public const string System = "system";

        #region HelpDesk
        static string[]? _allHelpDeskRoles = null;
        /// <summary>
        /// Все роли HelpDesk
        /// </summary>
        public static string[] AllHelpDeskRoles
        {
            get
            {
                _allHelpDeskRoles ??= [HelpDeskTelegramBotManager, HelpDeskTelegramBotUnit, HelpDeskTelegramBotRubricsManage, HelpDeskTelegramBotChatsManage];
                return _allHelpDeskRoles;
            }
        }

        /// <summary>
        /// Рубрики + <see cref="HelpDeskTelegramBotUnit"/> (таблица заявок клиентов связанных с текущим сотрудником)
        /// </summary>
        public const string HelpDeskTelegramBotRubricsManage = "HelpDeskTelegramBotRubricsManage";

        /// <summary>
        /// Чаты + <see cref="HelpDeskTelegramBotUnit"/> (таблица заявок клиентов связанных с текущим сотрудником)
        /// </summary>
        public const string HelpDeskTelegramBotChatsManage = "HelpDeskTelegramBotChatsManage";

        /// <summary>
        /// Консоль + <see cref="HelpDeskTelegramBotUnit"/> (таблица заявок клиентов связанных с текущим сотрудником)
        /// </summary>
        public const string HelpDeskTelegramBotManager = "HelpDeskTelegramBotManager";

        /// <summary>
        /// Таблица заявок клиентов связанных с текущим сотрудником
        /// </summary>
        public const string HelpDeskTelegramBotUnit = "HelpDeskTelegramBotUnit";

        /// <summary>
        /// CommerceManager
        /// </summary>
        public const string CommerceManager = "CommerceManager";
        #endregion
    }


    /// <summary>
    /// HelpdeskNotificationsTelegramAppName
    /// </summary>
    public const string HelpdeskNotificationsTelegramAppName = $"{Routes.HELPDESK_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}/{Routes.TELEGRAM_CONTROLLER_NAME}";


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