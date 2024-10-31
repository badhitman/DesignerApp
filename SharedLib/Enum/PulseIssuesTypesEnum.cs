////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

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
    Status = 0,

    /// <summary>
    /// Изменение в области темы, рубрики, описания
    /// </summary>
    Main = 10,

    /// <summary>
    /// Изменения связанные с сообщениями
    /// </summary>
    Messages = 20,

    /// <summary>
    /// Изменения исполнителя
    /// </summary>
    Executor = 30,

    /// <summary>
    /// Голосование
    /// </summary>
    Vote = 40,

    /// <summary>
    /// Изменения в подписчиках
    /// </summary>
    Subscribes = 50,

    /// <summary>
    /// Изменения в файлах
    /// </summary>
    Files = 60,
}