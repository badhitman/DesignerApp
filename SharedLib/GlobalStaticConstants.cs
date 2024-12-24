////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using System.Globalization;

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
    /// NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    /// </summary>
    public static JsonSerializerSettings JsonSerializerSettings
        => new() { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };


    /// <summary>
    /// Русская (ru-RU) CultureInfo
    /// </summary>
    public static readonly CultureInfo RU = CultureInfo.GetCultureInfo("ru-RU");


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
    public static string TransmissionQueueNamePrefix { get; set; } = "Transmission.Receives";

    /// <summary>
    /// DocumentName - property
    /// </summary>
    public const string DocumentNameProperty = "[DocumentNameProperty]";

    /// <summary>
    /// DocumentDate - property
    /// </summary>
    public const string DocumentDateProperty = "[DocumentDateProperty]";

    /// <summary>
    /// DocumentStatus - property
    /// </summary>
    public const string DocumentStatusProperty = "[DocumentStatusProperty]";

    /// <summary>
    /// HostAddress - property
    /// </summary>
    public const string HostAddressProperty = "[HostAddressProperty]";

    /// <summary>
    /// DocumentLinkAddressProperty
    /// </summary>
    public const string DocumentLinkAddressProperty = "[DocumentLinkAddressProperty]";

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
            PropertyName = Routes.PUBLIC_CONTROLLER_NAME,
            PrefixPropertyName = Routes.HTML_CONTROLLER_NAME,
        };

        /// <summary>
        /// HomePagePrivate
        /// </summary>
        public static StorageMetadataModel HomePagePrivate => new()
        {
            ApplicationName = Path.Combine(Routes.HOME_CONTROLLER_NAME, Routes.PAGE_CONTROLLER_NAME),
            PropertyName = Routes.PRIVATE_CONTROLLER_NAME,
            PrefixPropertyName = Routes.HTML_CONTROLLER_NAME,
        };

        /// <summary>
        /// TitleMain
        /// </summary>
        public static StorageMetadataModel TitleMain => new()
        {
            ApplicationName = Routes.MAIN_CONTROLLER_NAME,
            PropertyName = Routes.TITLE_CONTROLLER_NAME,
        };

        /// <summary>
        /// Отображение отключённых рубрик
        /// </summary>
        public static StorageMetadataModel ParameterShowDisabledRubrics => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.RUBRIC_CONTROLLER_NAME, Routes.FORM_CONTROLLER_NAME, $"{Routes.SHOW_ACTION_NAME}-{Routes.DISABLED_CONTROLLER_NAME}"),
        };

        /// <summary>
        /// Режим выбора рубрик в заявках
        /// </summary>
        public static StorageMetadataModel ModeSelectingRubrics => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.RUBRIC_CONTROLLER_NAME, Routes.FORM_CONTROLLER_NAME, $"{Routes.MODE_CONTROLLER_NAME}-{Routes.SELECT_ACTION_NAME}"),
        };

        /// <summary>
        /// Включение командного режима боту
        /// </summary>
        public static StorageMetadataModel ParameterIsCommandModeTelegramBot => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.TELEGRAM_CONTROLLER_NAME, Routes.COMMAND_CONTROLLER_NAME, Routes.MODE_CONTROLLER_NAME),
        };

        /// <summary>
        /// Отображать кнопку создания обращения
        /// </summary>
        public static StorageMetadataModel ShowCreatingIssue => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.CREATE_ACTION_NAME, Routes.COMMAND_CONTROLLER_NAME, Routes.ADD_ACTION_NAME),
        };

        /// <summary>
        /// Отображать колонку Claims в таблице пользователей
        /// </summary>
        public static StorageMetadataModel ShowClaimsUser => new()
        {
            ApplicationName = Routes.USERS_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.CLAIMS_CONTROLLER_NAME, Routes.SHOW_ACTION_NAME, Routes.COLUMN_CONTROLLER_NAME),
        };

        /// <summary>
        ///  Скрыть стоимость в списке оферов
        /// </summary>
        public static StorageMetadataModel HideWorthOffers => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = Path.Combine($"{Routes.WORTH_CONTROLLER_NAME}-{Routes.COLUMN_CONTROLLER_NAME}", Routes.OFFERS_CONTROLLER_NAME, Routes.HIDE_ACTION_NAME),
        };

        /// <summary>
        ///  Скрыть кратность в списке оферов
        /// </summary>
        public static StorageMetadataModel HideMultiplicityOffers => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = Path.Combine($"{Routes.MULTIPLICITY_CONTROLLER_NAME}-{Routes.COLUMN_CONTROLLER_NAME}", Routes.OFFERS_CONTROLLER_NAME, Routes.HIDE_ACTION_NAME),
        };

        /// <summary>
        /// ShowingTelegramArea
        /// </summary>
        public static StorageMetadataModel ShowingTelegramArea => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.SHOW_ACTION_NAME, Routes.TELEGRAM_CONTROLLER_NAME, Routes.AREAS_CONTROLLER_NAME),
        };

        /// <summary>
        /// ShowingWappiArea
        /// </summary>
        public static StorageMetadataModel ShowingWappiArea => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.SHOW_ACTION_NAME, Routes.WAPPI_CONTROLLER_NAME, Routes.AREAS_CONTROLLER_NAME),
        };

        /// <summary>
        /// ShowingAttachmentsOrderArea
        /// </summary>
        public static StorageMetadataModel ShowingAttachmentsOrderArea => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.SHOW_ACTION_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.ATTACHMENT_CONTROLLER_NAME, Routes.AREAS_CONTROLLER_NAME),
        };

        /// <summary>
        /// ShowingPriceSelectorOrder
        /// </summary>
        public static StorageMetadataModel ShowingPriceSelectorOrder => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.SHOW_ACTION_NAME, $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.PRICE_CONTROLLER_NAME}", $"{Routes.SELECT_ACTION_NAME}-{Routes.MODE_CONTROLLER_NAME}"),
        };

        /// <summary>
        /// ShowingAttachmentsIssuesArea
        /// </summary>
        public static StorageMetadataModel ShowingAttachmentsIssuesArea => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.SHOW_ACTION_NAME, Routes.ISSUE_CONTROLLER_NAME, Routes.ATTACHMENT_CONTROLLER_NAME, Routes.AREAS_CONTROLLER_NAME),
        };

        /// <summary>
        /// RubricIssueForCreateOrder
        /// </summary>
        public static StorageMetadataModel RubricIssueForCreateOrder => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.CREATE_ACTION_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.RUBRIC_CONTROLLER_NAME),
        };

        /// <summary>
        /// Уведомления Telegram о созданных заявках
        /// </summary>
        public static StorageMetadataModel HelpdeskNotificationTelegramForCreateIssue => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = $"{Routes.TELEGRAM_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}",
            PrefixPropertyName = $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.CREATE_ACTION_NAME}",
        };

        /// <summary>
        /// Настройка пересылки входящих сообщений в TelegramBot: глобально для пользователей на которых нет персональных подписок
        /// </summary>
        public static StorageMetadataModel HelpdeskNotificationTelegramGlobalForIncomingMessage => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = $"{Routes.TELEGRAM_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.GLOBAL_CONTROLLER_NAME,
        };

        /// <summary>
        /// WappiTokenApi
        /// </summary>
        public static StorageMetadataModel WappiTokenApi => new()
        {
            ApplicationName = Routes.WAPPI_CONTROLLER_NAME,
            PropertyName = $"{Routes.API_CONTROLLER_NAME}-{Routes.TOKEN_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.GLOBAL_CONTROLLER_NAME,
        };

        /// <summary>
        /// WappiTokenApi
        /// </summary>
        public static StorageMetadataModel WappiProfileId => new()
        {
            ApplicationName = Routes.WAPPI_CONTROLLER_NAME,
            PropertyName = $"{Routes.PROFILE_ACTION_NAME}-{Routes.IDENTITY_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.GLOBAL_CONTROLLER_NAME,
        };

        #region commerce notifications
        /// <summary>
        /// Тема уведомления при создании нового заказа
        /// </summary>
        public static StorageMetadataModel CommerceNewOrderSubjectNotification => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.CREATE_ACTION_NAME}",
            PrefixPropertyName = Routes.SUBJECT_CONTROLLER_NAME
        };

        /// <summary>
        /// Текст уведомления при создании нового заказа
        /// </summary>
        public static StorageMetadataModel CommerceNewOrderBodyNotification => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.CREATE_ACTION_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
        };


        /// <summary>
        /// Тема уведомления при изменении статуса заказа
        /// </summary>
        public static StorageMetadataModel CommerceStatusChangeOrderSubjectNotification(StatusesDocumentsEnum stage) => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.STATUS_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.SUBJECT_CONTROLLER_NAME,
            OwnerPrimaryKey = (int)stage
        };

        /// <summary>
        /// Текст уведомления при изменении статуса заказа
        /// </summary>
        public static StorageMetadataModel CommerceStatusChangeOrderBodyNotification(StatusesDocumentsEnum stage) => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.STATUS_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
            OwnerPrimaryKey = (int)stage
        };


        /// <summary>
        /// Тема уведомления при добавлении комментария к заказу
        /// </summary>
        public static StorageMetadataModel CommerceNewMessageOrderSubjectNotification => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.MESSAGE_CONTROLLER_NAME}-{Routes.ADD_ACTION_NAME}",
            PrefixPropertyName = Routes.SUBJECT_CONTROLLER_NAME,
        };

        /// <summary>
        /// Текст уведомления при добавлении комментария к заказу
        /// </summary>
        public static StorageMetadataModel CommerceNewMessageOrderBodyNotification => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.MESSAGE_CONTROLLER_NAME}-{Routes.ADD_ACTION_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
        };



        /// <summary>
        /// Whatsapp: Текст уведомления при создании нового заказа
        /// </summary>
        public static StorageMetadataModel CommerceNewOrderBodyNotificationWhatsapp => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.CREATE_ACTION_NAME}-{Routes.WHATSAPP_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
        };

        /// <summary>
        /// TELEGRAM: Текст уведомления при создании нового заказа
        /// </summary>
        public static StorageMetadataModel CommerceNewOrderBodyNotificationTelegram => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.CREATE_ACTION_NAME}-{Routes.TELEGRAM_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
        };

        /// <summary>
        /// TELEGRAM: Текст уведомления при изменении статуса заказа
        /// </summary>
        public static StorageMetadataModel CommerceStatusChangeOrderBodyNotificationTelegram(StatusesDocumentsEnum stepEnum) => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.STATUS_CONTROLLER_NAME}-{Routes.TELEGRAM_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
            OwnerPrimaryKey = (int)stepEnum
        };

        /// <summary>
        /// WHATSAPP: Текст уведомления при изменении статуса заказа
        /// </summary>
        public static StorageMetadataModel CommerceStatusChangeOrderBodyNotificationWhatsapp(StatusesDocumentsEnum stepEnum) => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.STATUS_CONTROLLER_NAME}-{Routes.WHATSAPP_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
            OwnerPrimaryKey = (int)stepEnum
        };

        /// <summary>
        /// TELEGRAM: Текст уведомления при добавлении комментария к заказу
        /// </summary>
        public static StorageMetadataModel CommerceNewMessageOrderBodyNotificationTelegram => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.MESSAGE_CONTROLLER_NAME}-{Routes.ADD_ACTION_NAME}-{Routes.TELEGRAM_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.BODY_CONTROLLER_NAME,
        };

        /// <summary>
        /// WHATSAPP: Текст уведомления при добавлении комментария к заказу
        /// </summary>
        public static StorageMetadataModel CommerceNewMessageOrderBodyNotificationWhatsapp => new()
        {
            ApplicationName = Routes.COMMERCE_CONTROLLER_NAME,
            PropertyName = $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}-{Routes.MESSAGE_CONTROLLER_NAME}-{Routes.ADD_ACTION_NAME}-{Routes.WHATSAPP_CONTROLLER_NAME}",
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
            PropertyName = Path.Combine(Routes.FILTER_CONTROLLER_NAME, user_id),
        };

        /// <summary>
        /// Уведомления в Telegram пользователю о событиях в его документах
        /// </summary>
        public static StorageMetadataModel NotificationTelegramForIssueUser(string user_id)
            => new()
            {
                ApplicationName = HelpdeskNotificationsTelegramAppName,
                PropertyName = Routes.USER_CONTROLLER_NAME,
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
            PropertyName = Path.Combine(Routes.CART_CONTROLLER_NAME, user_id),
        };

        /// <summary>
        /// Переадресация для пользователя
        /// </summary>
        public static StorageMetadataModel HelpdeskNotificationsTelegramForUser(long chat_id) => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = $"{Routes.TELEGRAM_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}/{Routes.USER_CONTROLLER_NAME}",
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

        /// <summary>
        /// Получить пользователей из Identity по их Email
        /// </summary>
        public readonly static string GetUsersOfIdentityByEmailReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.IDENTITY_CONTROLLER_NAME, Routes.USERS_CONTROLLER_NAME, $"{Routes.GET_ACTION_NAME}-by-{Routes.EMAIL_CONTROLLER_NAME}");

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
        public readonly static string SetWebConfigTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, $"{Routes.WEB_CONTROLLER_NAME}_{Routes.CONFIGURATION_CONTROLLER_NAME}", $"{Routes.SET_ACTION_NAME}-of-{Routes.TELEGRAM_CONTROLLER_NAME}");

        /// <inheritdoc/>
        public readonly static string SendWappiMessageReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.WAPPI_CONTROLLER_NAME, Routes.MESSAGE_CONTROLLER_NAME, Routes.SEND_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SetWebConfigHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, $"{Routes.WEB_CONTROLLER_NAME}_{Routes.CONFIGURATION_CONTROLLER_NAME}", $"{Routes.SET_ACTION_NAME}-of-{Routes.HELPDESK_CONTROLLER_NAME}");

        /// <inheritdoc/>
        public readonly static string SetWebConfigStorageReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, $"{Routes.WEB_CONTROLLER_NAME}_{Routes.CONFIGURATION_CONTROLLER_NAME}", $"{Routes.SET_ACTION_NAME}-of-{Routes.STORAGE_CONTROLLER_NAME}");
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
        public readonly static string StatusChangeOrderByHelpDeskDocumentIdReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.STATUS_CONTROLLER_NAME}-for-{Routes.ORDER_CONTROLLER_NAME}", $"{Routes.CHANGE_ACTION_NAME}-{Routes.UPDATE_ACTION_NAME}");

        /// <inheritdoc/>
        public readonly static string NomenclaturesReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.NOMENCLATURES_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string CalendarsSchedulesReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.WORKSCHEDULES_CONTROLLER_NAME}-{Routes.CALENDARS_CONTROLLER_NAME}", Routes.READ_ACTION_NAME);

        /// <summary>
        /// WorkSchedulesReadCommerceReceive
        /// </summary>
        public readonly static string WeeklySchedulesReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.WORKSCHEDULES_CONTROLLER_NAME}-{Routes.WEEKLY_CONTROLLER_NAME}", Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string NomenclaturesSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.NOMENCLATURES_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string WorksSchedulesFindCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.WORKSCHEDULES_CONTROLLER_NAME, Routes.FIND_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string CalendarsSchedulesSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.WORKSCHEDULES_CONTROLLER_NAME}-{Routes.CALENDARS_CONTROLLER_NAME}", Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string WeeklySchedulesSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.WORKSCHEDULES_CONTROLLER_NAME}-{Routes.WEEKLY_CONTROLLER_NAME}", Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrdersSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDERS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string WarehousesSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.WAREHOUSE_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OffersRegistersSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.OFFERS_CONTROLLER_NAME}-{Routes.REGISTERS_CONTROLLER_NAME}", Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrdersByIssuesGetReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.ORDERS_CONTROLLER_NAME}-by-{Routes.ISSUES_CONTROLLER_NAME}", Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrdersReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDERS_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrderReportGetCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.REPORT_CONTROLLER_NAME}", Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string PriceFullFileGetCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.PRICE_CONTROLLER_NAME}-{Routes.FULL_CONTROLLER_NAME}", Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string WarehousesDocumentsReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.WAREHOUSE_CONTROLLER_NAME}-{Routes.DOCUMENTS_CONTROLLER_NAME}", Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationsSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationsUsersSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.ORGANIZATIONS_CONTROLLER_NAME}-{Routes.USERS_CONTROLLER_NAME}", Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationsFindCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.FIND_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationsReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATION_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationsUsersReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.ORGANIZATIONS_CONTROLLER_NAME}-{Routes.USERS_CONTROLLER_NAME}", Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string AddressesOrganizationsReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ADDRESSES_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationUpdateOrCreateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATION_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationOfferContractUpdateOrCreateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.ORGANIZATION_CONTROLLER_NAME}-{Routes.OFFER_CONTROLLER_NAME}-{Routes.CONTRACT_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationUserUpdateOrCreateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.ORGANIZATION_CONTROLLER_NAME}-{Routes.USER_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationSetLegalCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.LEGAL_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string AddressOrganizationUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.ADDRESS_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <summary>
        /// Обновление номенклатуры
        /// </summary>
        public readonly static string NomenclatureUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.NOMENCLATURE_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string CalendarScheduleUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.WORKSCHEDULE_CONTROLLER_NAME}-{Routes.CALENDAR_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <summary>
        /// Attendance Update
        /// </summary>
        public readonly static string AttendanceUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ATTENDANCE_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <summary>
        /// WorkSchedule Update
        /// </summary>
        public readonly static string WeeklyScheduleUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, $"{Routes.WORKSCHEDULES_CONTROLLER_NAME}-{Routes.WEEKLY_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrderUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string WarehouseDocumentUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.WAREHOUSE_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <summary>
        /// Прикрепить файл к заказу (счёт, акт и т.п.)
        /// </summary>
        public readonly static string AttachmentAddToOrderCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.ATTACHMENT_CONTROLLER_NAME, Routes.ADD_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string AttachmentDeleteFromOrderCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.ATTACHMENT_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RowForOrderUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.ROW_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RowForWarehouseDocumentUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.WAREHOUSE_CONTROLLER_NAME, Routes.ROW_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string PaymentDocumentUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.PAYMENT_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string PaymentDocumentDeleteCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.PAYMENT_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RowsDeleteFromOrderCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.ROWS_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RowsDeleteFromWarehouseDocumentCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.WAREHOUSE_CONTROLLER_NAME, Routes.ROWS_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

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
        public readonly static string RubricsForArticleSetReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRICS_CONTROLLER_NAME}-for-{Routes.ARTICLES_CONTROLLER_NAME}", Routes.SET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricsForIssuesListHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRIC_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.LIST_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricForIssuesMoveHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRIC_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.MOVE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricForIssuesReadHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRIC_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricsForIssuesGetHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRIC_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.GET_ACTION_NAME);


        /// <inheritdoc/>
        public readonly static string IssueUpdateHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, Routes.ISSUE_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string IssuesSelectHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, Routes.ISSUE_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ArticlesSelectHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, Routes.ARTICLES_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

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
        public readonly static string TagsSelectReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.STORAGE_CONTROLLER_NAME, Routes.TAGS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string TagSetReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.STORAGE_CONTROLLER_NAME, Routes.TAG_CONTROLLER_NAME, Routes.SET_ACTION_NAME);

        /// <summary>
        /// Получить сводку (метаданные) по пространствам хранилища
        /// </summary>
        /// <remarks>
        /// Общий размер и количество группируется по AppName
        /// </remarks>
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
        public readonly static string ReadCloudParametersReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.CLOUD_CONTROLLER_NAME, Routes.PROPERTY_CONTROLLER_NAME, $"{Routes.READ_ACTION_NAME}-{Routes.LIST_ACTION_NAME}");

        /// <inheritdoc/>
        public readonly static string FindCloudParameterReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.CLOUD_CONTROLLER_NAME, Routes.PROPERTY_CONTROLLER_NAME, Routes.FIND_ACTION_NAME);
        #endregion
    }

    /// <summary>
    /// TelegramIdClaimName
    /// </summary>
    public readonly static string TelegramIdClaimName = Path.Combine(Routes.USER_CONTROLLER_NAME, Routes.TELEGRAM_CONTROLLER_NAME, Routes.IDENTITY_CONTROLLER_NAME);

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

        /// <summary>
        /// CommerceClient
        /// </summary>
        public const string CommerceClient = "CommerceClient";

        /// <summary>
        /// AttendancesExecutor
        /// </summary>
        public const string AttendancesExecutor = "AttendancesExecutor";
        #endregion
    }

    /// <summary>
    /// Cache
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// admin
        /// </summary>
        public static string ConsoleSegmentStatus(StatusesDocumentsEnum st) => $"{Routes.CONSOLE_CONTROLLER_NAME}:{Routes.SEGMENT_CONTROLLER_NAME}:{st}";

        /// <summary>
        /// ConsoleSegmentStatusToken
        /// </summary>
        public static MemCacheComplexKeyModel ConsoleSegmentStatusToken(StatusesDocumentsEnum st) => new(Routes.TOKEN_CONTROLLER_NAME, new MemCachePrefixModel(ConsoleSegmentStatus(st), Routes.SELECT_ACTION_NAME));
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