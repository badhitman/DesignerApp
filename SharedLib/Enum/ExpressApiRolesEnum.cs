using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Роли для экспресс авторизации (кастомная авторизация по токену)
/// </summary>
public enum ExpressApiRolesEnum
{
    /// <summary>
    /// TelegramBot
    /// </summary>
    [Description("TelegramBot - проверка пользователя")]
    TelegramBotUserCheck = 10,
}