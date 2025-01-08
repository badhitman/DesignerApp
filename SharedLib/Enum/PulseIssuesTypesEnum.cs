////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Типы событий мониторинга инцидентов
/// </summary>
public enum PulseIssuesTypesEnum
{
    /// <summary>
    /// Изменение статуса
    /// </summary>
    /// <remarks>
    /// При создании обращения это первое событие для установки начального статуса: Создан
    /// </remarks>
    [Description("Изменение статуса документа")]
    Status = 0,

    /// <summary>
    /// Изменение в области темы, рубрики, описания
    /// </summary>
    [Description("Изменение заявки")]
    Main = 10,

    /// <summary>
    /// Изменения связанные с сообщениями
    /// </summary>
    [Description("Сообщения в заявке")]
    Messages = 20,

    /// <summary>
    /// Изменения исполнителя
    /// </summary>
    [Description("Изменение исполнителя в заявке")]
    Executor = 30,

    /// <summary>
    /// Голосование
    /// </summary>
    [Description("Голосование в заявке")]
    Vote = 40,

    /// <summary>
    /// Изменения в подписчиках
    /// </summary>
    [Description("Изменения в подписчиках в заявке")]
    Subscribes = 50,

    /// <summary>
    /// Изменения в файлах
    /// </summary>
    [Description("Изменения в файлах в заявке")]
    Files = 60,

    /// <summary>
    /// Заказы
    /// </summary>
    Order = 70,

    /// <summary>
    /// Услуги/Бронь
    /// </summary>
    OrderAttendance = 80,
}