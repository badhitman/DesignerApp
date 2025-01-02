////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Globalization;
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
    /// TelegramIdClaimName
    /// </summary>
    public readonly static string TelegramIdClaimName = Path.Combine(Routes.USER_CONTROLLER_NAME, Routes.TELEGRAM_CONTROLLER_NAME, Routes.IDENTITY_CONTROLLER_NAME);

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