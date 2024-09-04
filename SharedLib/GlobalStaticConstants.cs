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
    /// Fake Host
    /// </summary>
    public const string FakeHost = "fake.null";

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
        /// <summary>
        /// Отображение отключёных рубрик
        /// </summary>
        public static StorageCloudParameterModel ParameterShowDisabledRubrics => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = Path.Combine(Routes.RUBRIC_CONTROLLER_NAME, Routes.FORM_CONTROLLER_NAME, $"{Routes.SHOW_ACTION_NAME}-{Routes.DISABLED_CONTROLLER_NAME}"),
        };

        /// <summary>
        /// Режим выбора рубрик в заявках
        /// </summary>
        public static StorageCloudParameterModel ModeSelectingRubrics => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = Path.Combine(Routes.RUBRIC_CONTROLLER_NAME, Routes.FORM_CONTROLLER_NAME, $"{Routes.MODE_ACTION_NAME}-{Routes.SELECT_ACTION_NAME}"),
        };

        /// <summary>
        /// Включение командного режима боту
        /// </summary>
        public static StorageCloudParameterModel ParameterIsCommandModeTelegramBot => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = Path.Combine(Routes.TELEGRAM_CONTROLLER_NAME, Routes.COMMAND_CONTROLLER_NAME, Routes.MODE_CONTROLLER_NAME),
        };

        /// <summary>
        /// Уведомления Telegram о созданных заявках
        /// </summary>
        public static StorageCloudParameterModel HelpdeskNotificationTelegramForCreateIssue => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = $"{Routes.TELEGRAM_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}",
            PrefixPropertyName = $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.CREATE_ACTION_NAME}",
        };

        /// <summary>
        /// Настройка пересылки входящих сообщений в TelegramBot: глобально для пользователей на которых нет персональных подписок
        /// </summary>
        public static StorageCloudParameterModel HelpdeskNotificationTelegramGlobalForIncomingMessage => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            Name = $"{Routes.TELEGRAM_CONTROLLER_NAME}-{Routes.NOTIFICATIONS_CONTROLLER_NAME}",
            PrefixPropertyName = Routes.GLOBAL_CONTROLLER_NAME,
        };

        /// <summary>
        /// Фильтр консоли по пользователю
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public static StorageCloudParameterModel ConsoleFilterForUser(string user_id) => new()
        {
            ApplicationName = Path.Combine(Routes.HELPDESK_CONTROLLER_NAME, Routes.CONSOLE_CONTROLLER_NAME),
            Name = Path.Combine(Routes.FILTER_CONTROLLER_NAME, user_id),
        };

        /// <summary>
        /// Переадресация для пользователя
        /// </summary>
        public static StorageCloudParameterModel HelpdeskNotificationsTelegramForUser(long chat_id) => new()
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
        /// <inheritdoc/>
        public readonly static string FindUsersOfIdentityReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.IDENTITY_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, Routes.FIND_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GetUsersOfIdentityReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.IDENTITY_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, Routes.GET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SelectUsersOfIdentityReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.IDENTITY_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SetRoleForUserOfIdentityReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.IDENTITY_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, Routes.ROLE_CONTROLLER_NAME, Routes.SET_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string FindUsersOfIdentityByTelegramIdsReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.IDENTITY_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, $"{Routes.FIND_ACTION_NAME}-{Routes.TELEGRAM_CONTROLLER_NAME}");

        /// <inheritdoc/>
        public readonly static string GetUsersOfIdentityByTelegramIdsReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.IDENTITY_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, $"{Routes.GET_ACTION_NAME}-{Routes.TELEGRAM_CONTROLLER_NAME}");

        /// <inheritdoc/>
        public readonly static string SendEmailReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.EMAIL_CONTROLLER_NAME, Routes.USER_CONTROLLER_NAME, Routes.FIND_ACTION_NAME);

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
        public readonly static string ForwardTextMessageTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.TEXT_CONTROLLER_NAME, Routes.MESSAGE_CONTROLLER_NAME, Routes.FORWARD_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ReadFileTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.FILE_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ChatsReadTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.CHATS_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ChatReadTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.CHAT_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ChatsFindForUserTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, $"{Routes.CHATS_CONTROLLER_NAME}-for-{Routes.USERS_CONTROLLER_NAME}", Routes.FIND_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string MessagesChatsSelectTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.MESSAGES_CONTROLLER_NAME, Routes.CHATS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ChatsSelectTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.CHATS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ErrorsForChatsSelectTelegramReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.TELEGRAM_CONTROLLER_NAME, Routes.ERRORS_CONTROLLER_NAME, Routes.CHATS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string SetWebConfigReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.WEB_CONTROLLER_NAME, Routes.CONFIGURATION_CONTROLLER_NAME, Routes.EDIT_ACTION_NAME);
        #endregion

        #region Commerce
        /// <inheritdoc/>
        public readonly static string GoodsSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.GOODS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrdersSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDERS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrdersReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDERS_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationsSelectCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationsReadCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATION_CONTROLLER_NAME, Routes.READ_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationUpdateOrCreateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrganizationSetLegalCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.LEGAL_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string AddressOrganizationUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.ADDRESS_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string GoodsUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.GOODS_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OrderUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RowForOrderUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.ROW_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string PaymentDocumentUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.PAYMENT_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RowsDeleteFromOrderCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.ROWS_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string DeliveryOrderUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.DELIVERY_CONTROLLER_NAME, Routes.ORDER_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OfferUpdateCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.GOODS_CONTROLLER_NAME, Routes.OFFER_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string AddressOrganizationDeleteCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.ORGANIZATIONS_CONTROLLER_NAME, Routes.ADDRESS_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string OfferDeleteCommerceReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.COMMERCE_CONTROLLER_NAME, Routes.OFFER_CONTROLLER_NAME, Routes.DELETE_ACTION_NAME);

        #endregion

        #region Helpdesk
        /// <inheritdoc/>
        public readonly static string RubricForIssuesUpdateHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.RUBRIC_CONTROLLER_NAME}", Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricsForIssuesListHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRIC_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.LIST_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricForIssuesMoveHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRIC_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.MOVE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string RubricForIssuesReadHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.RUBRIC_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.READ_ACTION_NAME);


        /// <inheritdoc/>
        public readonly static string IssueUpdateHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, Routes.ISSUE_CONTROLLER_NAME, Routes.UPDATE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string IssuesSelectHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-for-{Routes.USER_CONTROLLER_NAME}", Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string ConsoleIssuesSelectHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-{Routes.CONSOLE_CONTROLLER_NAME}", Routes.SELECT_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string StatusChangeIssueHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.STATUS_CONTROLLER_NAME}-for-{Routes.ISSUE_CONTROLLER_NAME}", Routes.CHANGE_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string PulseIssuePushHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, Routes.ISSUE_CONTROLLER_NAME, Routes.PULSE_CONTROLLER_NAME, Routes.PUSH_ACTION_NAME);

        /// <inheritdoc/>
        public readonly static string IssueGetHelpdeskReceive = Path.Combine(TransmissionQueueNamePrefix, Routes.HELPDESK_CONTROLLER_NAME, $"{Routes.ISSUE_CONTROLLER_NAME}-for-{Routes.USER_CONTROLLER_NAME}", Routes.READ_ACTION_NAME);

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
        /// Payment
        /// </summary>
        public const string PAYMENT_CONTROLLER_NAME = "payment";

        /// <summary>
        /// Row
        /// </summary>
        public const string ROW_CONTROLLER_NAME = "row";

        /// <summary>
        /// Rows
        /// </summary>
        public const string ROWS_CONTROLLER_NAME = "rows";

        /// <summary>
        /// Offer
        /// </summary>
        public const string OFFER_CONTROLLER_NAME = "offer";

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
        /// Проекты
        /// </summary>
        public const string PROJECTS_CONTROLLER_NAME = "projects";

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
        /// Filter
        /// </summary>
        public const string FILTER_CONTROLLER_NAME = "filter";

        /// <summary>
        /// Helpdesk
        /// </summary>
        public const string HELPDESK_CONTROLLER_NAME = "helpdesk";

        /// <summary>
        /// Commerce
        /// </summary>
        public const string COMMERCE_CONTROLLER_NAME = "commerce";

        /// <summary>
        /// Organization
        /// </summary>
        public const string ORGANIZATION_CONTROLLER_NAME = "organization";

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
        /// Disabled
        /// </summary>
        public const string DISABLED_CONTROLLER_NAME = "disabled";

        /// <summary>
        /// Issue
        /// </summary>
        public const string ISSUE_CONTROLLER_NAME = "issue";

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
        /// Web
        /// </summary>
        public const string WEB_CONTROLLER_NAME = "web";

        /// <summary>
        /// main
        /// </summary>
        public const string MAIN_CONTROLLER_NAME = "main";

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
        /// Role
        /// </summary>
        public const string ROLE_CONTROLLER_NAME = "role";

        /// <summary>
        /// Console
        /// </summary>
        public const string CONSOLE_CONTROLLER_NAME = "console";

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
        /// Переслать
        /// </summary>
        public const string FORWARD_ACTION_NAME = "forward";

        /// <summary>
        /// SET
        /// </summary>
        public const string SET_ACTION_NAME = "set";

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