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
        /// Отображение отключённых рубрик
        /// </summary>
        public static StorageMetadataModel ParameterEnabledWappi => new()
        {
            ApplicationName = Routes.WAPPI_CONTROLLER_NAME,
            PropertyName = Routes.ENABLED_CONTROLLER_NAME,
            PrefixPropertyName = Routes.GLOBAL_CONTROLLER_NAME,
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
        /// RubricIssueForCreateAttendanceOrder
        /// </summary>
        public static StorageMetadataModel RubricIssueForCreateAttendanceOrder => new()
        {
            ApplicationName = Routes.HELPDESK_CONTROLLER_NAME,
            PropertyName = Path.Combine(Routes.CREATE_ACTION_NAME, $"{Routes.ORDER_CONTROLLER_NAME}-{Routes.ATTENDANCE_CONTROLLER_NAME}", Routes.RUBRIC_CONTROLLER_NAME),
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
}