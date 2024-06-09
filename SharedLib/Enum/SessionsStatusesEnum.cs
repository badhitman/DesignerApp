using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Статусы сессий
/// </summary>
public enum SessionsStatusesEnum
{
    /// <summary>
    /// Сессия аннулирована
    /// </summary>
    [Description("Аннулировано")]
    None = 0,

    /// <summary>
    /// Заполнение данными
    /// </summary>
    [Description("Заполнение данными")]
    InProgress = 10,

    /// <summary>
    /// Отправлено на проверку
    /// </summary>
    [Description("На проверке")]
    Sended = 20,

    /// <summary>
    /// Принято
    /// </summary>
    [Description("Принято")]
    Accepted = 30
}