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
}