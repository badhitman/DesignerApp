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
        /// Загрузка TelegramBot WebApp
        /// </summary>
        public static StorageMetadataModel ParameterIncludeTelegramBotWebApp => new()
        {
            ApplicationName = Routes.TELEGRAM_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.WEB_CONTROLLER_NAME, Routes.APP_CONTROLLER_NAME, Routes.INCLUDE_ACTION_NAME),
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
        /// RubricIssueForCreateAttendance
        /// </summary>
        public static StorageMetadataModel RubricIssueForCreateAttendance => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.CREATE_ACTION_NAME, Routes.ATTENDANCES_CONTROLLER_NAME, Routes.RUBRIC_CONTROLLER_NAME),
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